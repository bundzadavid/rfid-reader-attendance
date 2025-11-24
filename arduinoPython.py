import time
import serial
import mysql.connector
from mysql.connector import Error
from datetime import datetime
from ArduinoGUI import *

# Funkcia na odoslanie dát cez sériový port do Arduina
def send_data(data):
    ser.write(bytes(data, 'utf-8'))  # konverzia reťazca na bajty a odoslanie

# Funkcia na aktualizáciu LCD displeja (meno + status dochádzky)
def update_lcd(name, attendance):
    send_data("!" + name + " $Had " + attendance)  # formátovaný text pre LCD
    time.sleep(1)  # krátka pauza, aby sa stihol zobraziť

# Funkcia na kontrolu stavu karty (ak má 3 absencie → Expired)
def check_card_status(uid):
    absence_query = "SELECT Absent_Count FROM students WHERE UID = %s"
    cursor.execute(absence_query, (uid,))
    absent_count = cursor.fetchone()

    if absent_count and absent_count[0] >= 3:
        status_query = "UPDATE students SET Attendance = 'Expired' WHERE UID = %s"
        cursor.execute(status_query, (uid,))
        db.commit()
        return True

    return False

# Pripojenie na sériový port (Arduino)
ser = serial.Serial('COM3', 9600)

# Pripojenie na MySQL databázu
try:
    db = mysql.connector.connect(
        host="localhost",
        user="root",
        password="",
        database="db_arduino"
    )
except Error as e:
    print("Error connecting to MySQL database:", e)

# Cursor na vykonávanie SQL príkazov
cursor = db.cursor()

# Vytvorenie tabuliek, ak ešte neexistujú
create_present_table = '''
CREATE TABLE IF NOT EXISTS Signin (
    Name VARCHAR(255),
    UID VARCHAR(255),
    Timestamp TIMESTAMP
)
'''
create_leave_table = '''
CREATE TABLE IF NOT EXISTS Signout (
    Name VARCHAR(255),
    UID VARCHAR(255),
    Timestamp TIMESTAMP
)
'''
create_interactions_table = '''
CREATE TABLE IF NOT EXISTS Students (
    Name VARCHAR(255),
    UID VARCHAR(255),
    Timestamp TIMESTAMP,
    Attendance VARCHAR(255),
    Present_Count INT,
    Leave_Count INT,
    Absent_Count INT
)
'''
try:
    cursor.execute(create_present_table)
    cursor.execute(create_leave_table)
    cursor.execute(create_interactions_table)
except Error as e:
    print("Error creating tables in the database:", e)

# Nekonečná slučka – čítanie dát zo sériového portu a aktualizácia DB
while True:
    line = ser.readline().decode().strip()  # načítaj riadok z Arduina
    if line.startswith('UID tag :'):
        # Extrahuj UID z riadku
        uid = line[10:].replace(' ', '')

        # Mapovanie UID na meno študenta
        if uid == '43B4C495':
            name = 'David B'
        elif uid == '81934043':
            name = 'Tomas K'
        elif uid == '63B001A6':
            name = 'Tomas G'
        elif uid == 'A1B2C3D4':   # nahraď reálnym UID Dominika
            name = 'Dominik V'
        else:
            name = 'Unknown'

        # Aktuálny čas
        timestamp = datetime.now()

        # Skontroluj, či už karta existuje v tabuľke Students
        match_query = "SELECT * FROM students WHERE UID = %s"
        cursor.execute(match_query, (uid,))
        match = cursor.fetchone()

        if not match:
            # Prvé priloženie karty → zapíš do Signin + Students
            present_query = "INSERT INTO Signin (Name, UID, Timestamp) VALUES (%s, %s, %s)"
            cursor.execute(present_query, (name, uid, timestamp))

            interactions_query = """INSERT INTO students 
                (Name, UID, Timestamp, Attendance, Present_Count, Leave_Count, Absent_Count) 
                VALUES (%s, %s, %s, %s, %s, %s, %s)"""
            cursor.execute(interactions_query, (name, uid, timestamp, 'Present', 1, 0, 0))

            # LCD výstup
            if check_card_status(uid):
                update_lcd(name, 'Expired')
            else:
                update_lcd(name, 'Signed In')
        else:
            # Karta už existuje → toggle medzi Present/Leave/Absent
            attendance = match[3]
            present_count = match[4]
            leave_count = match[5]
            absent_count = match[6]

            if attendance == 'Present':
                # Odchod → zapíš do Signout
                leave_query = "INSERT INTO Signout (Name, UID, Timestamp) VALUES (%s, %s, %s)"
                cursor.execute(leave_query, (name, uid, timestamp))
                attendance = 'Leave'
                leave_count += 1
                if check_card_status(uid):
                    update_lcd(name, 'Expired')
                else:
                    update_lcd(name, 'Signed Out')

            elif attendance == 'Absent':
                # Ak bol absent → teraz odchod
                leave_query = "INSERT INTO Signout (Name, UID, Timestamp) VALUES (%s, %s, %s)"
                cursor.execute(leave_query, (name, uid, timestamp))
                attendance = 'Leave'
                leave_count += 1
                if check_card_status(uid):
                    update_lcd(name, 'Expired')
                else:
                    update_lcd(name, 'Absent')

            else:
                # Opätovný príchod → zapíš do Signin
                attendance = 'Present'
                present_query = "INSERT INTO Signin (Name, UID, Timestamp) VALUES (%s, %s, %s)"
                cursor.execute(present_query, (name, uid, timestamp))
                present_count += 1
                if check_card_status(uid):
                    update_lcd(name, 'Expired')
                else:
                    update_lcd(name, 'Signed In')

            # Aktualizuj záznam v Students
            interactions_query = """UPDATE students 
                SET Attendance = %s, Present_Count = %s, Leave_Count = %s, Absent_Count = %s 
                WHERE UID = %s"""
            cursor.execute(interactions_query, (attendance, present_count, leave_count, absent_count, uid))

        # Kontrola absencie (ak študent neprišiel za posledných 24h)
        current_time = datetime.now()
        absent_query = "SELECT * FROM students WHERE Attendance = 'Present'"
        cursor.execute(absent_query)
        absent_students = cursor.fetchall()

        for student in absent_students:
            last_time = student[2]
            delta_time = current_time - last_time

            if delta_time.total_seconds() > 86400:  # 24 hodín
                attendance = 'Absent'
                interactions_query = """UPDATE students 
                    SET Attendance = %s, Present_Count = %s, Leave_Count = %s, Absent_Count = Absent_Count + 1 
                    WHERE UID = %s"""
                cursor.execute(interactions_query, (attendance, student[4], student[5], student[1]))
                check_card_status(student[1])

        # Ulož zmeny do DB
        db.commit()