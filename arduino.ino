#include <SPI.h>
#include <MFRC522.h>
#include <Wire.h>
#include <hd44780.h>                       // main hd44780 header
#include <hd44780ioClass/hd44780_I2Cexp.h> // i2c expander i/o class header




#define RST_PIN         2          // Configurable, see typical pin layout above
#define SS_PIN          10  
#define LED_G 5 //define green LED pin
#define LED_R 4 //define red LED pin
#define BUZZER 2 //buzzer pin
hd44780_I2Cexp lcd; // declare lcd object: auto locate & auto config expander chip

MFRC522 mfrc522(SS_PIN, RST_PIN);   
// LCD geometry
const int LCD_COLS = 16;
const int LCD_ROWS = 2;




      

int const RedLed=6;
int const GreenLed=5;
int const Buzzer=8;

bool showIDCard = true;
void setup() {
  int status;
  lcd.begin(16, 2);
  Serial.begin(9600); // Initialize serial communications with the PC
  SPI.begin();  // Init SPI bus
  mfrc522.PCD_Init(); // Init MFRC522 card
  status = lcd.begin(LCD_COLS, LCD_ROWS);
  pinMode(LED_G, OUTPUT);
pinMode(LED_R, OUTPUT);
pinMode(BUZZER, OUTPUT);
noTone(BUZZER);
  if(status) 
	{
		// hd44780 has a fatalError() routine that blinks an led if possible
		// begin() failed so blink error code using the onboard LED if possible
		hd44780::fatalError(status); // does not return
	}
  

  pinMode(RedLed,OUTPUT);
  pinMode(GreenLed,OUTPUT);
  pinMode(Buzzer,OUTPUT);
  
  lcd.setCursor(2,0);
  
  delay(200);
   }
   
void loop() {
  if (Serial.available() > 0) { // Read incoming data from Python
    String input = Serial.readStringUntil('\n'); // Read the incoming string

    int delimiterIndex = input.indexOf('$'); //separate the String recieved from the python to two parts that used & to separate the string 
    if (delimiterIndex != -1) {
      String firstLine = input.substring(1, delimiterIndex);
      String secondLine = input.substring(delimiterIndex + 1);

      lcd.clear();
      lcd.setCursor(0, 0); // Set cursor to the first line
      lcd.print(firstLine);

      lcd.setCursor(0, 1); // Set cursor to the second line
      lcd.print(secondLine);
      delay(1000);
      lcd.clear();
      
      
    }
  }

  if (showIDCard) {
    if ( ! mfrc522.PICC_IsNewCardPresent()) {
      
       
      lcd.setCursor(3, 0);  
      lcd.print("SHOW YOUR");  
      lcd.setCursor(4, 1);  
      lcd.print("ID CARD");   
    } else {
      lcd.clear();
    }
  }

  if ( ! mfrc522.PICC_ReadCardSerial()) {
    return;
  }

  // Show UID on serial monitor
  Serial.print("UID tag :");
  String content = "";
  
  for (byte i = 0; i < mfrc522.uid.size; i++) {
    Serial.print(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " ");
    Serial.print(mfrc522.uid.uidByte[i], HEX);
    content.concat(String(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " "));
    content.concat(String(mfrc522.uid.uidByte[i], HEX));
  }
  
  Serial.println();
  content.toUpperCase();
  
  if (!showIDCard) {
    lcd.clear();
    lcd.setCursor(0, 1);
    lcd.print(content);
  }
}


