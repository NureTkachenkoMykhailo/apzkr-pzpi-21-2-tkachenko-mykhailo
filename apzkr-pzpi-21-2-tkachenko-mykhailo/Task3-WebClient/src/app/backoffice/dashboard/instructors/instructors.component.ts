import { Component, OnInit } from '@angular/core';
import { InstructorService } from "../../../core/services/instructors.service";

@Component({
  selector: 'app-instructors',
  templateUrl: './instructors.component.html',
  styleUrls: ['./instructors.component.css']
})
export class InstructorsComponent implements OnInit {
  columns = [
    { key: 'id', label: 'ID' },
    { key: 'firstName', label: 'First Name' },
    { key: 'lastName', label: 'Last Name' },
    { key: 'email', label: 'Email' },
  ];

  data: any[] = [];

  actions = [
    {
      label: 'Delete',
      callback: (row: any) => this.onDeleteInstructor(row)
    }
  ];

  constructor(private instructorService: InstructorService) {}

  ngOnInit(): void {
    this.loadInstructors();
  }

  loadInstructors(): void {
    this.instructorService.getInstructors().subscribe(
      data => {
        this.data = data;
      },
      error => {
        console.error('Error fetching instructors', error);
        alert('Failed to load instructors.');
      }
    );
  }

  deleteInstructor(id: number): void {
    if (confirm('Are you sure you want to delete this instructor?')) {
      this.instructorService.deleteInstructor(id).subscribe(
        () => {
          this.data = this.data.filter(i => i.id !== id);
          alert('Instructor deleted successfully.');
        },
        error => {
          console.error('Error deleting instructor', error);
          alert('Failed to delete instructor.');
        }
      );
    }
  }

  onDeleteInstructor(row: any) {
    this.deleteInstructor(row.id);
  }
}
