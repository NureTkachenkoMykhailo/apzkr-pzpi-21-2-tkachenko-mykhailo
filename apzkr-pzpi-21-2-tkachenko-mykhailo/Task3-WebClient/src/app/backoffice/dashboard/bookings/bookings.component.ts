import { Component } from '@angular/core';

@Component({
  selector: 'app-bookings',
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.css'
})
export class BookingsComponent {
  title='Dashboard'
  columns = [
    { key: 'id', label: 'ID' },
    { key: 'date', label: 'Date' },
    { key: 'status', label: 'Status' }
  ];

  data = [
    { id: 1, date: '2024-08-19', status: 'Confirmed' },
    { id: 2, date: '2024-07-19', status: 'Confirmed' },
    { id: 3, date: '2024-07-21', status: 'Cancelled' }
  ];
}
