// fetch calendar events from backend
export async function fetchCalendarEvents({ take, language, campus, audience } = {}) {
  const baseUrl = process.env.REACT_APP_API_URL || '';
  let url = baseUrl ? baseUrl + '/calendar-events' : '/calendar-events';
  const params = [];
  if (take) params.push(`take=${take}`);
  if (language) params.push(`language=${language}`);
  if (campus) params.push(`campus=${campus}`);
  if (audience) params.push(`audience=${audience}`);
  if (params.length) url += '?' + params.join('&');

  const res = await fetch(url);
  if (!res.ok) throw new Error('Failed to fetch events');
  // The backend returns a JSON string, so parse twice
  const text = await res.text();
  try {
    return JSON.parse(text);
  } catch {
    return [];
  }
}
