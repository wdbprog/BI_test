import React, { useEffect, useState } from 'react';
import './App.css';
import { fetchCalendarEvents } from './api';

function App() {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchCalendarEvents()
      .then(data => {
        // If backend returns an array, use it; if string, parse it
        let parsed = data;
        if (typeof data === 'string') {
          try { parsed = JSON.parse(data); } catch { parsed = []; }
        }
        setEvents(parsed);
        setLoading(false);
      })
      .catch(e => {
        setError(e.message);
        setLoading(false);
      });
  }, []);

  return (
    <div className="App">
      <h1>BI Calendar Events</h1>
      {loading && <p>Loading events...</p>}
      {error && <p style={{color:'red'}}>Error: {error}</p>}
      <ul style={{listStyle:'none',padding:0}}>
        {Array.isArray(events) && events.map(ev => (
          <li key={ev.id} style={{marginBottom:24,border:'1px solid #ccc',borderRadius:8,padding:16}}>
            <h2>{ev.title}</h2>
            {ev.imageUrl && <img src={ev.imageUrl} alt={ev.imageText||ev.title} style={{maxWidth:200}} />}
            <div><b>Location:</b> {ev.location}</div>
            <div><b>Start:</b> {ev.startTime} <b>End:</b> {ev.endTime}</div>
            <div><b>Audience:</b> {ev.filterList}</div>
            <a href={ev.url} target="_blank" rel="noopener noreferrer">Event Link</a>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;
