import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Training } from "../../../../core/models/training";

@Component({
  selector: 'app-training-details-dialog',
  templateUrl: './training-details-dialog.component.html',
  styleUrl: './training-details-dialog.component.css'
})
export class TrainingDetailsDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public training: Training) {
    this.levels = training.levels.map(l => l.name ?? l.level.toString());
  }

  levels: string[] = [];
}
