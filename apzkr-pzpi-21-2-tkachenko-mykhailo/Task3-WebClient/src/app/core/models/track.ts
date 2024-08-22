export interface Track {
  id?: number;
  name: string;
  sections: Section[];
}

export interface Section {
  id?: number;
  trackId?: number;
  information: SectionInfo;
  factors: SectionFactors;
}

export interface SectionInfo {
  name: string;
  curvatureDegrees: number;
  danger: DangerLevel
}

export interface SectionFactors {
  wind: SectionFactor;
  snow: SectionFactor;
  iciness: SectionFactor;
}

export interface SectionFactor {
  key: string,
  multiplicationValue: number
}

export enum DangerLevel{
  noDanger = 0,
  mediocre = 10,
  dangerous = 20,
  veryDangerous = 30,
  exceptionalDanger = 40
}
