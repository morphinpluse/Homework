// hal konande maze az har noghte shoro
// sensor pins
const int leftSensorPin = 2;
const int frontSensorPin = 3;
const int rightSensorPin = 4;

// motor pins
const int leftMotorForward = 5;
const int leftMotorBackward = 6;
const int rightMotorForward = 7;
const int rightMotorBackward = 8;

void setup() {
  // sesor input
  pinMode(leftSensorPin, INPUT);
  pinMode(frontSensorPin, INPUT);
  pinMode(rightSensorPin, INPUT);

  // motor output
  pinMode(leftMotorForward, OUTPUT);
  pinMode(leftMotorBackward, OUTPUT);
  pinMode(rightMotorForward, OUTPUT);
  pinMode(rightMotorBackward, OUTPUT);
}

void loop() {
  if (pathOnRight()) {
    turnRight();
    delay(1000); 

  } else if (pathAhead()) {
    moveForward();
    delay(500); 

  } else if (pathOnLeft()) {
    turnLeft();
    delay(1000);
    
  } else {
    turnAround();
    delay(2000); 
  }
}

bool pathOnRight() {
  return digitalRead(rightSensorPin) == HIGH;
}

bool pathAhead() {
  return digitalRead(frontSensorPin) == HIGH;
}

bool pathOnLeft() {
  return digitalRead(leftSensorPin) == HIGH;
}

void turnRight() {
  digitalWrite(leftMotorForward, HIGH);
  digitalWrite(rightMotorBackward, HIGH);
}

void moveForward() {
  digitalWrite(leftMotorForward, HIGH);
  digitalWrite(rightMotorForward, HIGH);
}

void turnLeft() {
  digitalWrite(leftMotorBackward, HIGH);
  digitalWrite(rightMotorForward, HIGH);
}

void turnAround() {
  digitalWrite(leftMotorBackward, HIGH);
  digitalWrite(rightMotorBackward, HIGH);
}
