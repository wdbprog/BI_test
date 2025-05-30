import React, { useEffect, useState } from 'react';
import './App.css';
import { fetchCalendarEvents } from './api';
import type { CalendarEvent } from './models';

function App() {
  const [events, setEvents] = useState([] as CalendarEvent[]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  // Filter state
  const [take, setTake] = useState<number>(5);
  const [language, setLanguage] = useState<string>('all');
  const [campus, setCampus] = useState<string>('');
  const [audience, setAudience] = useState<string>('');

  // Fetch events with filters
  const fetchEvents = () => {
    setLoading(true);
    setError(null);
    fetchCalendarEvents({ take, language, campus, audience })
      .then((data: CalendarEvent[] | string) => {
        let parsed: CalendarEvent[] = Array.isArray(data) ? data : [];
        if (typeof data === 'string') {
          try { parsed = JSON.parse(data); } catch { parsed = []; }
        }
        setEvents(parsed);
        setLoading(false);
      })
      .catch((e: any) => {
        setError(e.message);
        setLoading(false);
      });
  };

  useEffect(() => {
    fetchEvents();
    // eslint-disable-next-line
  }, []);

  // UI for filters
  return (
    <div className="App">
      <h1>BI Calendar Events</h1>
      <form
        style={{marginBottom:24,display:'flex',gap:12,flexWrap:'wrap',alignItems:'center'}}
        onSubmit={e => { e.preventDefault(); fetchEvents(); }}
      >
        <label>
          Take:
          <input type="number" min={1} max={50} value={take} onChange={e => setTake(Number(e.target.value))} style={{width:60,marginLeft:4}} />
        </label>
        <label>
          Language:
          <select value={language} onChange={e => setLanguage(e.target.value)} style={{marginLeft:4}}>
            <option value="all">All</option>
            <option value="no">Norwegian</option>
            <option value="en">English</option>
          </select>
        </label>
        <label>
          Campus:
          <input type="text" value={campus} onChange={e => setCampus(e.target.value)} placeholder="Campus name" style={{marginLeft:4}} />
        </label>
        <label>
          Audience:
          <input type="text" value={audience} onChange={e => setAudience(e.target.value)} placeholder="Audience" style={{marginLeft:4}} />
        </label>
        <button type="submit">Apply Filters</button>
      </form>
      {loading && <p>Loading events...</p>}
      {error && <p style={{color:'red'}}>Error: {error}</p>}
      <ul style={{listStyle:'none',padding:0}}>
        {Array.isArray(events) && events.map(ev => (
          <li key={ev.id} style={{marginBottom:24,border:'1px solid #ccc',borderRadius:8,padding:16,display:'flex',alignItems:'flex-start',gap:16}}>
            {ev.imageUrl && (
              <img src={ev.imageUrl} alt={ev.imageText||ev.title} style={{maxWidth:160, maxHeight:120, objectFit:'cover', borderRadius:6}} />
            )}
            <div style={{flex:1}}>
              <h2>{ev.title}</h2>
              <div><b>Location:</b> {ev.location}</div>
              <div><b>Start:</b> {ev.startTime} <b>End:</b> {ev.endTime}</div>
              <div><b>Audience:</b> {ev.filterList}</div>
              <a href={ev.url} target="_blank" rel="noopener noreferrer">Event Link</a>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;
