import { randomUUID } from "node:crypto";
import { Dino, Diet, seedDinos } from "./dinos.js";

export interface NewDinoInput {
  name: string;
  species: string;
  period: string;
  diet: Diet;
  lengthMeters: number;
  emoji?: string;
  funFact?: string;
}

const DIETS: Diet[] = ["herbivoro", "carnivoro", "onivoro"];

export class DinoStore {
  private dinos: Dino[];

  constructor(initial: Dino[] = seedDinos) {
    this.dinos = initial.map((d) => ({ ...d }));
  }

  list(): Dino[] {
    return this.dinos.map((d) => ({ ...d }));
  }

  get(id: string): Dino | undefined {
    const found = this.dinos.find((d) => d.id === id);
    return found ? { ...found } : undefined;
  }

  add(input: NewDinoInput): Dino {
    const errors = validateNewDino(input);
    if (errors.length > 0) {
      throw new ValidationError(errors);
    }

    const dino: Dino = {
      id: randomUUID(),
      name: input.name.trim(),
      species: input.species.trim(),
      period: input.period.trim(),
      diet: input.diet,
      lengthMeters: input.lengthMeters,
      emoji: input.emoji?.trim() || "🦕",
      funFact: input.funFact?.trim() || "Um novo habitante do Mundo Dino!",
    };
    this.dinos.push(dino);
    return { ...dino };
  }
}

export class ValidationError extends Error {
  constructor(public readonly errors: string[]) {
    super(errors.join("; "));
    this.name = "ValidationError";
  }
}

export function validateNewDino(input: Partial<NewDinoInput>): string[] {
  const errors: string[] = [];
  if (!input.name || !input.name.trim()) {
    errors.push("O campo 'name' e obrigatorio.");
  }
  if (!input.species || !input.species.trim()) {
    errors.push("O campo 'species' e obrigatorio.");
  }
  if (!input.period || !input.period.trim()) {
    errors.push("O campo 'period' e obrigatorio.");
  }
  if (!input.diet || !DIETS.includes(input.diet)) {
    errors.push(`O campo 'diet' deve ser um de: ${DIETS.join(", ")}.`);
  }
  if (
    typeof input.lengthMeters !== "number" ||
    Number.isNaN(input.lengthMeters) ||
    input.lengthMeters <= 0
  ) {
    errors.push("O campo 'lengthMeters' deve ser um numero maior que zero.");
  }
  return errors;
}
