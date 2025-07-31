# OpenWeatherMap API Endpoints Documentation

## Implemented API Endpoints

### 1. Geocoding API (Direct Geocoding)
**Endpoint:** `https://api.openweathermap.org/geo/1.0/direct`

**Implementation in WeatherService.cs:42**
```
geo/1.0/direct?q={city}&limit={limit}&appid={API key}
```

**Parameters:**
- `q` (required): City name, state code (only for the US) and country code divided by comma
- `limit` (optional): Number of the locations in the API response (up to 5 results can be returned)
- `appid` (required): Your unique API key

**Example Request:**
```
https://api.openweathermap.org/geo/1.0/direct?q=London,GB&limit=5&appid={API key}
```

**Response Format:**
```json
[
  {
    "name": "London",
    "local_names": {...},
    "lat": 51.5073219,
    "lon": -0.1276474,
    "country": "GB"
  }
]
```

**Status:** ✅ Correctly implemented

---

### 2. One Call API 3.0 (Current Weather, Forecast, Alerts)
**Endpoint:** `https://api.openweathermap.org/data/3.0/onecall`

**Implementation in WeatherService.cs:111**
```
data/3.0/onecall?lat={latitude}&lon={longitude}&units={units}&lang={language}&appid={API key}
```

**Parameters:**
- `lat` (required): Latitude, decimal (-90; 90)
- `lon` (required): Longitude, decimal (-180; 180)
- `units` (optional): Units of measurement (`standard`, `metric`, `imperial`)
- `lang` (optional): Language parameter for localized output
- `exclude` (optional): Comma-delimited list to exclude parts of weather data
- `appid` (required): Your unique API key

**Available exclude values:**
- `current` - Current weather
- `minutely` - Minute forecast for 1 hour
- `hourly` - Hourly forecast for 48 hours
- `daily` - Daily forecast for 8 days
- `alerts` - Government weather alerts

**Example Requests:**
```
# Current weather only (exclude all forecasts and alerts)
https://api.openweathermap.org/data/3.0/onecall?lat=33.44&lon=-94.04&exclude=minutely,hourly,daily,alerts&appid={API key}

# Weather forecast (exclude current and alerts)
https://api.openweathermap.org/data/3.0/onecall?lat=33.44&lon=-94.04&exclude=current,minutely,alerts&appid={API key}

# Weather alerts only (exclude all weather data)
https://api.openweathermap.org/data/3.0/onecall?lat=33.44&lon=-94.04&exclude=current,minutely,hourly,daily&appid={API key}
```

**Status:** ✅ Correctly implemented

---

## API Usage in Project

### GetCurrentWeather
**Handler:** `GetCurrentWeatherHandler.cs`
**API Call:** Uses `exclude=minutely,hourly,daily,alerts` to get only current weather

### GetWeatherForecast  
**Handler:** `GetWeatherForecastHandler.cs`
**API Call:** Uses `exclude=minutely,current,alerts` to get hourly and daily forecasts

### GetWeatherAlerts
**Handler:** `GetWeatherAlertsHandler.cs`
**API Call:** Uses `exclude=minutely,hourly,daily,current` to get only alerts

---

## Response Data Structure

### Current Weather Response
```json
{
  "lat": 33.44,
  "lon": -94.04,
  "timezone": "America/Chicago",
  "current": {
    "dt": 1684929490,
    "sunrise": 1684926645,
    "sunset": 1684977332,
    "temp": 292.55,
    "feels_like": 292.87,
    "pressure": 1014,
    "humidity": 89,
    "clouds": 53,
    "visibility": 10000,
    "wind_speed": 3.58,
    "wind_deg": 152,
    "weather": [
      {
        "id": 500,
        "main": "Rain",
        "description": "light rain",
        "icon": "10d"
      }
    ]
  }
}
```

### Daily Forecast Response
```json
{
  "daily": [
    {
      "dt": 1684951200,
      "sunrise": 1684926645,
      "sunset": 1684977332,
      "temp": {
        "day": 299.03,
        "min": 290.69,
        "max": 300.35,
        "night": 291.45,
        "eve": 297.51,
        "morn": 292.55
      },
      "weather": [
        {
          "id": 500,
          "main": "Rain",
          "description": "light rain",
          "icon": "10d"
        }
      ]
    }
  ]
}
```

### Weather Alerts Response
```json
{
  "alerts": [
    {
      "sender_name": "NWS Philadelphia",
      "event": "Small Craft Advisory",
      "start": 1684952747,
      "end": 1684988747,
      "description": "...SMALL CRAFT ADVISORY REMAINS IN EFFECT...",
      "tags": []
    }
  ]
}
```

---

## API Key Requirements

- **One Call API 3.0** requires a "One Call by Call" subscription
- **Geocoding API** is included in the free tier
- 1,000 free daily API calls included
- API responses are updated every 10 minutes

All implemented API endpoints match the official OpenWeatherMap One Call API 3.0 documentation.