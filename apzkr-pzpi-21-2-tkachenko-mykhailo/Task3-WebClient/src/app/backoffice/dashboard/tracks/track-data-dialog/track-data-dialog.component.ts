import {Component, Inject, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import { TrackService } from "../../../../core/services/tracks.service";
import { Track } from "../../../../core/models/track";

@Component({
  selector: 'app-track-data-dialog',
  templateUrl: './track-data-dialog.component.html',
  styleUrls: ['./track-data-dialog.component.css']
})
export class TrackDataDialogComponent implements OnInit {
  trackForm: FormGroup;

  dangerLevels = [
    { value: 0, label: 'No Danger' },
    { value: 10, label: 'Mediocre' },
    { value: 20, label: 'Dangerous' },
    { value: 30, label: 'Very Dangerous' },
    { value: 40, label: 'Exceptional Danger' }
  ];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<TrackDataDialogComponent>,
    private trackService: TrackService,
    @Inject(MAT_DIALOG_DATA) public track: Track
  )
  {
    this.trackForm = this.fb.group({
      name: ['', Validators.required],
      sections: this.fb.array([])
    });

    if (this.track) {
      this.populateForm(this.track);
    }
  }

  populateForm(track: Track): void {
    this.trackForm.patchValue({
      name: track.name
    });

    track.sections.forEach(section => {
      const sectionGroup = this.fb.group({
        informationName: [section.information.name, Validators.required],
        informationCurvatureDegrees: [section.information.curvatureDegrees, [Validators.required, Validators.min(0)]],
        informationDefaultDangerLevel: [section.information.danger, [Validators.required]],
        factorsWind: [section.factors.wind.multiplicationValue, [Validators.required, Validators.min(0)]],
        factorsSnow: [section.factors.snow.multiplicationValue, [Validators.required, Validators.min(0)]],
        factorIciness: [section.factors.iciness.multiplicationValue, [Validators.required, Validators.min(0)]],
      });

      this.sections.push(sectionGroup);
    });
  }

  ngOnInit(): void {
    if (this.sections.length === 0){
      this.addSection();
    }
  }

  get sections(): FormArray {
    return this.trackForm.get('sections') as FormArray;
  }

  addSection(): void {
    const sectionGroup = this.fb.group({
      informationName: ['', Validators.required],
      informationCurvatureDegrees: [0, [Validators.required, Validators.min(0)]],
      informationDefaultDangerLevel:  [0, [Validators.required]],
      factorsWind:  [0, [Validators.required, Validators.min(0)]],
      factorsSnow: [0, [Validators.required, Validators.min(0)]],
      factorIciness: [0, [Validators.required, Validators.min(0)]],
    });

    this.sections.push(sectionGroup);
  }

  removeSection(index: number): void {
    if (this.sections.length > 1) {
      this.sections.removeAt(index);
    }
  }

  createTrack(): void {
    if (this.trackForm.valid) {
      const formValue = this.trackForm.value;
      const track: Track = {
        id: this.track?.id ?? 0,
        name: formValue.name,
        sections: formValue.sections.map((section: any) => ({
          information: {
            name: section.informationName,
            curvatureDegrees: section.informationCurvatureDegrees,
            danger: section.informationDefaultDangerLevel,
          },
          factors: {
            wind: section.factorsWind,
            snow: section.factorsSnow,
            iciness: section.factorIciness
          }
        }))
      };

      if (this.track) {
        this.trackService.updateTrack(track.id ?? -1, track).subscribe(
          () => this.dialogRef.close(true),
          error => console.error('Error updating track:', error)
        );
      } else {
        this.trackService.createTrack(track).subscribe(
          () => this.dialogRef.close(true),
          error => console.error('Error creating track:', error)
        );
      }
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
