#!/usr/bin/env python3
"""
Simple test script to verify Weather MCP Server functionality
"""
import json
import subprocess
import sys
import os

def send_mcp_request(request):
    """Send MCP request to the server and get response"""
    try:
        # Start the MCP server process
        env = os.environ.copy()
        env['OPENWEATHER_API_KEY'] = 'bd5e378503939ddaee76f12ad7a97608'
        
        process = subprocess.Popen(
            ['dotnet', 'run', '--project', 'WeatherMcpServer/WeatherMcpServer.csproj'],
            stdin=subprocess.PIPE,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            env=env
        )
        
        # Send request
        request_json = json.dumps(request) + '\n'
        stdout, stderr = process.communicate(input=request_json, timeout=10)
        
        print(f"Request: {request}")
        print(f"Response: {stdout}")
        if stderr:
            print(f"Errors: {stderr}")
        
        return stdout, stderr
        
    except Exception as e:
        print(f"Error: {e}")
        return None, str(e)

def test_list_tools():
    """Test listing available tools"""
    request = {
        "jsonrpc": "2.0",
        "id": 1,
        "method": "tools/list"
    }
    return send_mcp_request(request)

def test_current_weather():
    """Test GetCurrentWeather tool"""
    request = {
        "jsonrpc": "2.0",
        "id": 2,
        "method": "tools/call",
        "params": {
            "name": "GetCurrentWeather",
            "arguments": {
                "city": "Moscow",
                "countryCode": "RU"
            }
        }
    }
    return send_mcp_request(request)

def test_weather_forecast():
    """Test GetWeatherForecast tool"""
    request = {
        "jsonrpc": "2.0",
        "id": 3,
        "method": "tools/call",
        "params": {
            "name": "GetWeatherForecast",
            "arguments": {
                "city": "London",
                "countryCode": "UK",
                "days": 3
            }
        }
    }
    return send_mcp_request(request)

if __name__ == "__main__":
    print("=== Testing Weather MCP Server ===\n")
    
    print("1. Testing tools list...")
    test_list_tools()
    
    print("\n2. Testing current weather...")
    test_current_weather()
    
    print("\n3. Testing weather forecast...")
    test_weather_forecast()
    
    print("\n=== Test completed ===")
