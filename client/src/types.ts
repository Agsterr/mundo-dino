export type Diet = "herbivoro" | "carnivoro" | "onivoro";

export interface Dino {
  id: string;
  name: string;
  species: string;
  period: string;
  diet: Diet;
  lengthMeters: number;
  emoji: string;
  funFact: string;
}

export interface NewDinoInput {
  name: string;
  species: string;
  period: string;
  diet: Diet;
  lengthMeters: number;
  emoji?: string;
  funFact?: string;
}
