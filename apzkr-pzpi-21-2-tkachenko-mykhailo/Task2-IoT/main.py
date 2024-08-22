import ujson
import urequests
import time

from MathService import MathService
from NetworkService import NetworkService
from ConfigurationService import ConfigurationService
from HazardService import HazardService

def configure_network():
    while True:
        print("\nConfigure Network Settings:")
        print("1. SSID Name: {}".format(ConfigurationService.Networking.SSID_NAME))
        print("2. WiFi Password: {}".format(ConfigurationService.Networking.PASSWORD))
        print("3. Back")
        choice = input("Choose an option to configure or go back: ")
        
        if choice == '1':
            ssid_name = input("Enter new SSID Name: ")
            ConfigurationService.update_networking(ssid_name, ConfigurationService.Networking.PASSWORD)
        elif choice == '2':
            password = input("Enter new WiFi Password: ")
            ConfigurationService.update_networking(ConfigurationService.Networking.SSID_NAME, password)
        elif choice == '3':
            break
        else:
            print("Invalid option, please choose again.")

def configure_backend():
    while True:
        print("\nConfigure Backend Settings:")
        print("1. Backend URL: {}".format(ConfigurationService.Communications.Backend.BACKEND_URL))
        print("2. Auth Token: {}".format(ConfigurationService.Communications.Backend.AUTH_TOKEN))
        print("3. Back")
        choice = input("Choose an option to configure or go back: ")
        
        if choice == '1':
            backend_url = input("Enter new Backend URL: ")
            ConfigurationService.update_backend(backend_url, ConfigurationService.Communications.Backend.AUTH_TOKEN)
        elif choice == '2':
            auth_token = input("Enter new Auth Token: ")
            ConfigurationService.update_backend(ConfigurationService.Communications.Backend.BACKEND_URL, auth_token)
        elif choice == '3':
            break
        else:
            print("Invalid option, please choose again.")

def configure_track_section_id():
    while True:
        print("\nConfigure Track Section ID:")
        track_section_id = input("Enter new Track Section ID (current: {}): ".format(ConfigurationService.TRACK_SECTION_ID))
        if track_section_id.lower() == "back":
            break
        if track_section_id.isdigit():
            ConfigurationService.update_track_section_id(int(track_section_id))
            break
        else:
            print("Invalid Track Section ID. Please enter a numeric value or type 'back' to return.")

def configure():
    while True:
        print("\nConfiguration Menu:")
        print("1. Configure Network")
        print("2. Configure Backend")
        print("3. Configure Track Section ID")
        print("4. Back to Main Menu")
        choice = input("Choose an option: ")
        
        if choice == '1':
            configure_network()
        elif choice == '2':
            configure_backend()
        elif choice == '3':
            configure_track_section_id()
        elif choice == '4':
            break
        else:
            print("Invalid option, please choose again.")

def main_loop():
    network_service = NetworkService()
    sta_if = network_service.connect_to_wifi(
        ConfigurationService.Networking.SSID_NAME, 
        ConfigurationService.Networking.PASSWORD)

    data_service = HazardService()
    last_pressure = MathService.normal_distribution(
        ConfigurationService.Simulation.Pressure.DEFAULT_PRESSURE, 
        ConfigurationService.Simulation.Pressure.DEFAULT_DISTRIBUTION)

    while True:
        try:
            print("Measuring weather conditions... ", end="")
            temperature, humidity, current_pressure = data_service.read_sensors()
            hazard_data = data_service.calculate_danger_index(temperature, humidity, current_pressure, last_pressure)
            message_type = data_service.determine_message_type(hazard_data)
            data = {
                "trackSectionId": ConfigurationService.TRACK_SECTION_ID,
                "messageType": message_type,
                "message": "exceptional weather conditions detected" if message_type == 3 else "weather conditions warning" if message_type == 2 else "adequate weather conditions",
                "metadata": hazard_data
            }

            headers = {
                'Content-Type': 'application/json',
                'Authorization': ConfigurationService.Communications.Backend.AUTH_TOKEN
            }
            response = urequests.post(
                ConfigurationService.Communications.Backend.BACKEND_URL, 
                data= ujson.dumps(data), 
                headers=headers)
            print("Posted to backend: ", response.text)
            response.close()

            last_pressure = current_pressure
        except OSError as e:
            print("Failed to send data: ", e)
        except Exception as e:
            print("Error: ", e)
        
        time.sleep(10)

def main():
    while True:
        print("\nMain Menu:")
        print("1. Configure Settings")
        print("2. Start Data Sending")
        choice = input("Choose an option: ")
        
        if choice == '1':
            configure()
        elif choice == '2':
            main_loop()
        else:
            print("Invalid option, please choose again.")

if __name__ == "__main__":
    main()
