class ConfigurationService:
    class Communications:
        class Backend:
            BACKEND_URL = "http://194.1.220.48:5298/communications/iot"
            AUTH_TOKEN = "362EA6716B524D718A65C30D7284A682"
    class Simulation:
        class Pressure:
            DEFAULT_PRESSURE = 1015
            DEFAULT_DISTRIBUTION = 5
    class Networking:
        SSID_NAME = 'Wokwi-GUEST'
        PASSWORD = ''
    TRACK_SECTION_ID = 5

    @classmethod
    def update_networking(cls, ssid_name, password):
        cls.Networking.SSID_NAME = ssid_name
        cls.Networking.PASSWORD = password

    @classmethod
    def update_backend(cls, backend_url, auth_token):
        cls.Communications.Backend.BACKEND_URL = backend_url
        cls.Communications.Backend.AUTH_TOKEN = auth_token

    @classmethod
    def update_track_section_id(cls, track_section_id):
        cls.TRACK_SECTION_ID = track_section_id
    