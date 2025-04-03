import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService, CalendarEvent } from '../services/event.service';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements OnInit {
  days: number[] = [];
  events: CalendarEvent[] = [];
  showNewEventForm = false;
  newEvent: Partial<CalendarEvent> = {};
  
  // The currentDate controls the month/year displayed.
  currentDate: Date = new Date();

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

  openNewEventForm(): void {
    this.showNewEventForm = true;
    this.newEvent = {};
  }

  addEvent(): void {
    let localBegin: Date;
    let localEnd: Date;
    if (!this.newEvent.name || !this.newEvent.begin || !this.newEvent.end) {
      alert('Please fill in required fields');
      return;
    }

    // Validate and convert newEvent.begin
    if (this.newEvent.begin == null) {
      alert("Please provide Begin value");
      return;
    } else if (typeof this.newEvent.begin === 'string') {
      localBegin = new Date(this.newEvent.begin);
    } else if (this.newEvent.begin instanceof Date) {
      localBegin = this.newEvent.begin;
    } else {
      throw new Error("Unexpected type for newEvent.begin");
    }

    // Validate and convert newEvent.end
    if (this.newEvent.end == null) {
      alert("Please provide End value");
      return;
    } else if (typeof this.newEvent.end === 'string') {
      localEnd = new Date(this.newEvent.end);
    } else if (this.newEvent.end instanceof Date) {
      localEnd = this.newEvent.end;
    } else {
      throw new Error("Unexpected type for newEvent.end");
    }

    // Convert local dates to UTC dates
    const utcBegin = new Date(
      localBegin.getTime() - localBegin.getTimezoneOffset() * 60000
    );
    const utcEnd = new Date(
      localEnd.getTime() - localEnd.getTimezoneOffset() * 60000
    );

    // Assign the converted UTC dates back to newEvent
    this.newEvent.begin = utcBegin;
    this.newEvent.end = utcEnd;

    this.eventService.createEvent(this.newEvent).subscribe(() => {
      this.loadEvents();
      this.showNewEventForm = false;
    });
  }

  deleteEvent(eventId: number): void {
    if (confirm("Are you sure you want to delete this event?")) {
      this.eventService.deleteEvent(eventId).subscribe(() => {
        this.loadEvents();
      });
    }
  }
}
