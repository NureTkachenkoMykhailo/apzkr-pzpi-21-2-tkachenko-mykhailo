from machine import Pin
import dht
from MathService import MathService

class HazardService:
    def __init__(self):
        self.sensor = dht.DHT22(Pin(15))

    def read_sensors(self):
        self.sensor.measure()
        temperature = self.sensor.temperature()
        humidity = self.sensor.humidity()
        pressure = MathService.normal_distribution(1015, 5)
        return temperature, humidity, pressure

    def calculate_iciness_hazard(self, temp, hum):
        if temp < -10 or (temp <= 0 and hum > 90):
            return 30, "Severe cold, high risk of icing"
        elif -10 <= temp <= 0:
            return 20, "Cold conditions, risk of snow and ice"
        else:
            return 10, "Low risk of icing"

    def calculate_wind_hazard(self, current_press, last_press):
        pressure_change = abs(current_press - last_press)
        if pressure_change > 5:
            return 30, "Severe wind gusts expected"
        elif pressure_change > 2:
            return 20, "Moderate wind conditions"
        else:
            return 10, "Stable wind conditions"

    def calculate_snow_hazard(self, temp, hum, press):
        if temp <= 0 and hum > 70:
            if press < 1010:
                return "30", "High chance of snow"
            else:
                return "20", "Moderate chance of snow"
        else:
            return 10, "Low chance of snow"

    def calculate_danger_index(self, temp, hum, press, last_press):
        I_score, I_message = self.calculate_iciness_hazard(temp, hum)
        W_score, W_message = self.calculate_wind_hazard(press, last_press)
        S_score, S_message = self.calculate_snow_hazard(temp, hum, press)

        hazard_data = {
            "iciness": I_score,
            "wind": W_score,
            "snow": S_score,
        }

        return hazard_data

    def determine_message_type(self, hazard_data):
        max_hazard = max(hazard_data.values())
        if max_hazard <= 10:
            return 0
        elif max_hazard <= 20:
            return 1
        elif max_hazard <= 30:
            return 2
        else:
            return 3
