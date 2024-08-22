import network
import time

class NetworkService:
    @staticmethod
    def connect_to_wifi(ssid, password):
        print("Connecting to WiFi", end="")
        sta_if = network.WLAN(network.STA_IF)
        sta_if.active(True)
        sta_if.connect(ssid, password)
        while not sta_if.isconnected():
            print(".", end="")
            time.sleep(0.1)
        print(" Connected!")
        return sta_if
