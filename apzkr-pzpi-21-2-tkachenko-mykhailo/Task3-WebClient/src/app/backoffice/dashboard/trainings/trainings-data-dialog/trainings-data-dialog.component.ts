import {Component, Inject, OnInit} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {TrackService} from "../../../../core/services/tracks.service";
import {InstructorService} from "../../../../core/services/instructors.service";
import {Track} from "../../../../core/models/track";
import {Identity} from "../../../../core/models/identity";
import {Level, Training} from "../../../../core/models/training";
import {TrainingService} from "../../../../core/services/trainings.service";
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-trainings-data-dialog',
  templateUrl: './trainings-data-dialog.component.html',
  styleUrl: './trainings-data-dialog.component.css'
})
export class TrainingsDataDialogComponent implements OnInit {
  trainingForm: FormGroup;
  tracks: Track[] = [];
  instructors: Identity[] = [];
  levels: Level[] = [
    {
      name: "junior",
      level: 0
    } ,
    {
      name: "middle",
      level: 1
    },
    {
      name: "senior",
      level: 2
    }];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<TrainingsDataDialogComponent>,
    private trackService: TrackService,
    private instructorService: InstructorService,
    private trainingService: TrainingService,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public existingTraining?: Training
  ) {
    this.trainingForm = this.fb.group({
      trackId: [null, Validators.required],
      instructorId: [null, Validators.required],
      levels: this.levels,
      generalInformationName: ['', Validators.required],
      generalInformationDescriptions: ['', Validators.required],
      generalInformationMinutes: [30, Validators.required],
      generalInformationStart : [new Date(), Validators.required],
    });

    if (this.existingTraining){
      this.patchTrainingData(existingTraining as Training);
    }
  }

  ngOnInit(): void {
    this.loadTracks();
    this.loadInstructors();
  }

  loadTracks(): void {
    this.trackService.getTracks().subscribe(data => {
      this.tracks = data;
    });
  }

  loadInstructors(): void {
    this.instructorService.getInstructors().subscribe(data => {
      this.instructors = data;
    });
  }

  saveTraining(): void {
    if (this.trainingForm.valid) {
      const training: Training = {
        id: this.existingTraining?.id || 0,
        trackId: this.trainingForm.value.trackId,
        instructorId: this.trainingForm.value.instructorId,
        levels: this.trainingForm.value.levels.map((l: any) => ({ level: l })),
        information: {
          name: this.trainingForm.value.generalInformationName,
          descriptions: this.trainingForm.value.generalInformationDescriptions,
          durationMinutes: this.trainingForm.value.generalInformationMinutes,
          start: this.trainingForm.value.generalInformationStart,
        }
      };

      const request$ = this.existingTraining
        ? this.trainingService.updateTraining(training.id ?? -1, training)
        : this.trainingService.createTraining(training);

      request$.subscribe(
        () => {
          const successMessage = this.existingTraining
            ? 'Training updated successfully!'
            : 'Training created successfully!';

          this.snackBar.open(successMessage, 'Close', {
            duration: 3000,
            verticalPosition: 'top',
            panelClass: ['success-snackbar']
          });
          this.dialogRef.close(true);
        },
        (error) => {
          console.log(error);
          let errorMessage = 'Failed to save training. Please try again.';
          if (error.status === 401 || error.status === 403) {
            errorMessage = "No rights";
          }
          this.snackBar.open(errorMessage, 'Close', {
            duration: 3000,
            verticalPosition: 'top',
            panelClass: ['error-snackbar']
          });
          this.dialogRef.close(false);
        }
      );
    }
  }

  patchTrainingData(training: Training): void {
    this.trainingForm.patchValue({
      trackId: training.trackId,
      instructorId: training.instructorId,
      generalInformationName: training.information.name,
      generalInformationDescriptions: training.information.descriptions,
      generalInformationMinutes: training.information.durationMinutes,
      generalInformationStart: training.information.start,
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
