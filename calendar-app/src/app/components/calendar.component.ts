import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService, CalendarEvent } from '../services/event.service';
import { catchError, EMPTY, tap } from 'rxjs';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements OnInit {
  days: number[] = []; // Represents each day of the active month
  events: CalendarEvent[] = [];
  showNewEventForm = false;
  newEvent: Partial<CalendarEvent> = {};
  
  // The currentDate controls the month/year displayed.
  currentDate: Date = new Date();

  // Holds an event being edited
  editingEvent: CalendarEvent | null = null;

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.updateCalendar();
    this.loadEvents();
  }

  // Updates the days array based on the current month and year.
  updateCalendar(): void {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    this.days = Array.from({ length: daysInMonth }, (_, i) => i + 1);
  }

  // Next month button.
  nextMonth(): void {
    this.currentDate = new Date(
      this.currentDate.getFullYear(),
      this.currentDate.getMonth() + 1,
      1
    );
    this.updateCalendar();
  }

  // Previous month button.
  prevMonth(): void {
    this.currentDate = new Date(
      this.currentDate.getFullYear(),
      this.currentDate.getMonth() - 1,
      1
    );
    this.updateCalendar();
  }

  // Populate events array from api response
  loadEvents(): void {
    this.eventService.getEvents().subscribe(data => {
      // Ensure date strings are converted to Date objects.
      this.events = data.events.map(e => ({
        ...e,
        begin: new Date(e.begin),
        end: new Date(e.end)
      }));
    });
  }

  // Use currentDate month and year to check if an event exists on a given day.
  hasEvent(day: number): boolean {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();
    return this.events.some(e => {
      const eventDate = new Date(e.begin);
      return eventDate.getFullYear() === year &&
             eventDate.getMonth() === month &&
             eventDate.getDate() === day;
    });
  }

  // Returns the events for the given day based on the current month/year.
  getEventsForDay(day: number): CalendarEvent[] {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();
    return this.events.filter(e => {
      const eventDate = new Date(e.begin);
      return eventDate.getFullYear() === year &&
             eventDate.getMonth() === month &&
             eventDate.getDate() === day;
    });
  }

  // Controls viewing of new event form
  openNewEventForm(): void {
    this.showNewEventForm = true;
    this.newEvent = {};
  }

  // Adds a new event
  addEvent(): void {
    if (!this.processEventDates(this.newEvent)) {
      return;
    }

    this.eventService.createEvent(this.newEvent).pipe(
      tap(() => {
        this.loadEvents();
        this.showNewEventForm = false;
      }),
      catchError(error => {
        alert(this.extractErrorMessage(error));
        return EMPTY;
      })
    ).subscribe();
  }

  // Open the edit form with the selected event details.
  openEditEventForm(event: CalendarEvent): void {
    // Create a copy to avoid modifying the event directly
    this.editingEvent = { ...event };
  }

  // Updates an existing event
  updateEvent(): void {
    if (!this.editingEvent) return;

    if (!this.processEventDates(this.editingEvent)) {
      return;
    }

    this.eventService.updateEvent(this.editingEvent).pipe(
      tap(() => {
        this.loadEvents();
        this.editingEvent = null;
      }),
      catchError(error => {
        alert(this.extractErrorMessage(error));
        return EMPTY;
      })
    ).subscribe();
  }

  // Allows edit to be cancelled
  cancelEdit(): void {
    this.editingEvent = null;
  }  

  // Deletes the given event
  deleteEvent(eventId: number): void {
    if (confirm("Are you sure you want to delete this event?")) {
      this.eventService.deleteEvent(eventId).subscribe(() => {
        this.loadEvents();
      });
    }
  }

  // Helper method to validate and convert begin and end dates
  private processEventDates(event: Partial<CalendarEvent>): boolean {
    let localBegin: Date;
    let localEnd: Date;

    // Check for required fields
    if (
      !event.name ||
      !event.begin ||
      Object.keys(event.begin).length === 0 ||
      !event.end ||
      Object.keys(event.end).length === 0
    ) {
      alert('Please fill in required fields');
      return false;
    }

    // Process begin date
    if (typeof event.begin === 'string') {
      localBegin = new Date(event.begin);
    } else if (event.begin instanceof Date) {
      localBegin = event.begin;
    } else {
      alert("Unexpected type for event.begin");
      return false;
    }

    // Process end date
    if (typeof event.end === 'string') {
      localEnd = new Date(event.end);
    } else if (event.end instanceof Date) {
      localEnd = event.end;
    } else {
      alert("Unexpected type for event.end");
      return false;
    }

    // Convert local dates to UTC dates
    event.begin = new Date(
      localBegin.getTime() - localBegin.getTimezoneOffset() * 60000
    );
    event.end = new Date(
      localEnd.getTime() - localEnd.getTimezoneOffset() * 60000
    );

    return true;
  }

  // Helper method to extract FastEndpoint GeneralError messages from the error response
  private extractErrorMessage(error: any): string {
    let errorMessage = 'An unexpected error occurred';
    if (
      error.error &&
      error.error.errors &&
      error.error.errors.generalErrors &&
      Array.isArray(error.error.errors.generalErrors)
    ) {
      errorMessage = error.error.errors.generalErrors.join(', ');
    } else if (error.error && typeof error.error === 'string') {
      errorMessage = error.error;
    } else if (error.error && error.error.message) {
      errorMessage = error.error.message;
    } else {
      errorMessage = JSON.stringify(error.error);
    }
    return errorMessage;
  }
}
