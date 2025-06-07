import axios from 'axios';
import React, { useState } from 'react';
import {
  WiCloud,
  WiRain,
  WiDaySunny,
  WiSnow,
  WiThunderstorm,
  WiFog,
  WiNa,
} from 'react-icons/wi';

function Weather() {
  const [location, setLocation] = useState('');
  const [weatherData, setWeatherData] = useState(null);
  const [historyData, setHistoryData] = useState([]);
  const [notFound, setNotFound] = useState(false);
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');

  const getWeatherIcon = (description, size = 70) => {
    if (!description) return <WiNa size={size} />;

    const desc = description.toLowerCase();
    if (desc.includes('cloud')) return <WiCloud size={size} />;
    if (desc.includes('rain')) return <WiRain size={size} />;
    if (desc.includes('clear')) return <WiDaySunny size={size} />;
    if (desc.includes('snow')) return <WiSnow size={size} />;
    if (desc.includes('thunderstorm')) return <WiThunderstorm size={size} />;
    if (desc.includes('fog') || desc.includes('mist')) return <WiFog size={size} />;

    return <WiNa size={size} />;
  };

  async function load() {
    try {
      const result = await axios.get(`https://localhost:7293/api/Weather/current/${location}`);
      setWeatherData(result.data);
      setNotFound(false);
    } catch (error) {
      console.error('Axios error:', error);
      setNotFound(true);
      setWeatherData(null);
    }
  }

  async function load2() {
    if (!location || !fromDate || !toDate) {
      alert('Please enter location, from date, and to date.');
      return;
    }

    try {
      const result = await axios.get(`https://localhost:7293/api/Weather/bydate`, {
        params: {
          location: location,
          from: fromDate,
          to: toDate,
        },
      });

      setHistoryData(result.data);
    } catch (error) {
      console.error('Range fetch error:', error.response?.data || error.message);
      setHistoryData([]);
    }
  }

  function handleSubmit(e) {
    e.preventDefault();
    if (location.trim() !== '') {
      load();
    }
  }

  function formatDate(dateString) {
    if (!dateString) return 'Date not available';
    const date = new Date(dateString);
    return date.toLocaleString();
  }

  return (
    <div className="container py-4">
      <form onSubmit={handleSubmit} className="row mb-4">
        <div className="col-md-3 mb-2">
          <input
            type="text"
            className="form-control"
            placeholder="Enter location"
            value={location}
            onChange={(e) => setLocation(e.target.value)}
          />
        </div>
        <div className="col-md-2 mb-2">
          <input
            type="date"
            className="form-control"
            value={fromDate}
            onChange={(e) => setFromDate(e.target.value)}
          />
        </div>
        <div className="col-md-2 mb-2">
          <input
            type="date"
            className="form-control"
            value={toDate}
            onChange={(e) => setToDate(e.target.value)}
          />
        </div>
        <div className="col-md-2 mb-2">
          <button type="submit" className="btn btn-primary w-100">
            Current
          </button>
        </div>
        <div className="col-md-2 mb-2">
          <button type="button" className="btn btn-secondary w-100" onClick={load2}>
            History
          </button>
        </div>
      </form>

      <div className="row">
        <div className="col-md-6">
          {notFound && <p className="text-danger">Location not found or error fetching data.</p>}

          {weatherData && !notFound && (
            <div className="shadow-lg rounded-4 p-4 bg-white">
              <p className="fw-bold">Today in {weatherData.location}</p>
              <div className="d-flex align-items-center">
                <h1 className="display-3">{weatherData.temperatureCelsius.toFixed(1)}°C</h1>
                <div style={{ marginLeft: 15 }}>
                  {getWeatherIcon(weatherData.weatherDescription, 70)}
                </div>
              </div>
              <h2 className="text-warning text-capitalize">{weatherData.weatherDescription}</h2>
              <p>{formatDate(weatherData.date)}</p>
            </div>
          )}
        </div>

        <div className="right-panel glass rounded-4 p-3 w-50">
          <div className="d-flex justify-content-between text-center">
            {historyData.slice(0, 7).map((item, index) => (
              <div key={index} className="hour-card">
                <p className="small mb-1">{new Date(item.date).getHours()}:00</p>
                <div>{getWeatherIcon(item.weatherDescription, 40)}</div>
                <p className="small mb-0">{item.temperatureCelsius.toFixed(0)}°</p>
              </div>
            ))}
          </div>
          <div className="mt-4">
            <h6 className="mb-2">Summary</h6>
            <p className="small text-light-emphasis">
              Weather history for the selected date shows variable temperatures throughout the
              morning.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Weather;
