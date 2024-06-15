//ultra sonic pins 
const int trigPin = 9;
const int echoPin = 10;
//ultra sonic
long duration;
int distance;

const int irSensor = 2; // IR sensor pin
int irValue;
//Gyrooscope
#include <Gyroscope.h>
Gyroscope gyro;
// motors
Servo motor1;
Servo motor2;

bool roomCleaned = false;
void setup() {
pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  motor1.attach(3); 
  motor2.attach(5); 
// ir pinmodes
  pinMode(irSensor, INPUT);
  gyro.begin(); // Initialize gyroscope

}

void loop() {
  // sensor read
 digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  
  duration = pulseIn(echoPin, HIGH);
  distance = duration * 0.034 / 2;
  // ir sensor
 irValue = digitalRead(irSensor);
  if (distance < 20) {
    //  detected within 20 cm
    motor1.write(0); 
    motor2.write(180); 
    delay(500);
  } else {
    //move forward
    motor1.write(180);
    motor2.write(180);
  }
 if (irValue == HIGH) {
    // Detected finer obstacle, change direction
    motor1.write(0);
    motor2.write(180);
    delay(500);
    roomCleaned = true;
  }


}
