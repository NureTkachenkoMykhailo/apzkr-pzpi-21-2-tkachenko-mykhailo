import {Track} from "./track";
import {Identity} from "./identity";
import {TrainingInformation} from "./trainingInformation";

export interface Training {
  id?: number;
  trackId: number;
  instructorId: number;
  levels: Level[];
  information: TrainingInformation;
  track?: Track;
  instructor?: Identity;
}

export interface Level {
  level: number,
  name?: string
}
