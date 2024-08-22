import { Component, OnInit } from "@angular/core";
import { Training } from "../../../core/models/training";
import { MatDialog } from "@angular/material/dialog";
import { TrainingService } from "../../../core/services/trainings.service";
import { TrainingDetailsDialogComponent } from "./training-details-dialog/training-details-dialog.component";
import {TrainingsDataDialogComponent} from "./trainings-data-dialog/trainings-data-dialog.component";
import { MatSnackBar } from "@angular/material/snack-bar";

@Component({
  selector: 'app-trainings',
  templateUrl: './trainings.component.html',
  styleUrls: ['./trainings.component.css']
})
export class TrainingsComponent implements OnInit {
  columns = [
    { key: 'id', label: 'ID' },
    { key: 'information.name', label: 'Name' },
    { key: 'track.name', label: 'Track' },
    { key: 'instructor.firstName', label: 'Instructor' },
    { key: 'information.start', label: 'Start' },
  ];

  data: Training[] = [];

  actions = [
    {
      label: 'Delete',
      callback: (row: Training) => {
        this.deleteTraining(row.id ?? -1);
      }
    },
    {
      label: 'Update',
      callback: (row: Training) => {
        this.openDataTrainingDialog(row)
      }
    }
  ];

  constructor(
    private trainingService: TrainingService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadTrainingSessions();
  }

  loadTrainingSessions(): void {
    this.trainingService.getTrainings().subscribe(data => {
      this.data = data;
      console.log(data);
    });
  }

  openDataTrainingDialog(training?: Training): void {
    console.log("data for dialog: ", training);
    const dialogRef = this.dialog.open(TrainingsDataDialogComponent, {
      width: '600px',
      data: training
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.loadTrainingSessions();
      }
    });
  }

  deleteTraining(trainingId: number): void {
    this.trainingService.deleteTraining(trainingId).subscribe(
      success => {
        const successMessage: string = "Successfully deleted training " + trainingId;
        console.log(successMessage);
        this.snackBar.open(successMessage, 'Close', {
          duration: 3000,
          verticalPosition: 'top',
          panelClass: ['success-snackbar']
        });
      },
      error => {
        const errorMessage: string = "Could not delete training with id " + trainingId;
        console.log(errorMessage)
        this.snackBar.open(errorMessage, 'Close', {
          duration: 3000,
          verticalPosition: 'top',
          panelClass: ['error-snackbar']
        });
      }
    )
  }

  onRowClick(training: Training): void {
    const dialogRef = this.dialog.open(TrainingDetailsDialogComponent, {
      width: '600px',
      height: '400px',
      data: training
    });
  }
}
