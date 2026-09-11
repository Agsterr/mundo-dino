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

export const seedDinos: Dino[] = [
  {
    id: "trex",
    name: "Rex",
    species: "Tyrannosaurus rex",
    period: "Cretaceo",
    diet: "carnivoro",
    lengthMeters: 12,
    emoji: "🦖",
    funFact: "Sua mordida era uma das mais fortes que ja existiram na Terra.",
  },
  {
    id: "brachio",
    name: "Bruno",
    species: "Brachiosaurus",
    period: "Jurassico",
    diet: "herbivoro",
    lengthMeters: 22,
    emoji: "🦕",
    funFact: "Alcancava as copas das arvores mais altas com seu pescoco enorme.",
  },
  {
    id: "tricera",
    name: "Cera",
    species: "Triceratops",
    period: "Cretaceo",
    diet: "herbivoro",
    lengthMeters: 9,
    emoji: "🦏",
    funFact: "Tinha tres chifres e um grande escudo osseo para se defender.",
  },
  {
    id: "raptor",
    name: "Blue",
    species: "Velociraptor",
    period: "Cretaceo",
    diet: "carnivoro",
    lengthMeters: 2,
    emoji: "🦎",
    funFact: "Cacava em bando e era extremamente agil e inteligente.",
  },
  {
    id: "stego",
    name: "Spike",
    species: "Stegosaurus",
    period: "Jurassico",
    diet: "herbivoro",
    lengthMeters: 9,
    emoji: "🐢",
    funFact: "As placas em suas costas ajudavam a regular a temperatura do corpo.",
  },
];
