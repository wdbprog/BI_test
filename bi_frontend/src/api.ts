import type { CalendarEvent } from './models';

//api call to backend to fetch calendar events
export async function fetchCalendarEvents({ take, language, campus, audience }:{ 
        take?: number; 
        language?: string; 
        campus?: string; 
        audience?: string 
    } = {}): Promise<CalendarEvent[]> {
  
   //get base URL from env
  const baseUrl = process.env.REACT_APP_API_URL || '';
  let url = baseUrl ? baseUrl + '/calendar-events' : '/calendar-events';
  
  //build parameters
  const params: string[] = [];
  if (take) params.push(`take=${take}`);
  if (language) params.push(`language=${language}`);
  if (campus) params.push(`campus=${campus}`);
  if (audience) params.push(`audience=${audience}`);
  if (params.length) url += '?' + params.join('&');

  const res = await fetch(url);
  if (!res.ok) throw new Error('Failed to fetch events');

  const text = await res.text();
  try {
    return JSON.parse(text) as CalendarEvent[];
  } catch {
    return [];
  }
}

export {};
