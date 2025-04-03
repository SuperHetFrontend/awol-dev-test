import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

// TODO: Transfer the models to their own classes
export interface CalendarEvent {
  id: number;
  name: string;
  description: string;
  begin: Date;
  end: Date;
}

interface EventsResponse {
  events: CalendarEvent[];
}

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = 'http://localhost:5233/api/events';

  constructor(private http: HttpClient) {}

  getEvents(): Observable<EventsResponse> {
    return this.http.get<EventsResponse>(this.apiUrl);
  }

  createEvent(event: Partial<CalendarEvent>): Observable<CalendarEvent> {
    return this.http.post<CalendarEvent>(this.apiUrl, event);
  }

  updateEvent(event: CalendarEvent): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${event.id}`, event);
  }

  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}