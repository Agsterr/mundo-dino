import { Dino, NewDinoInput } from "./types";

export async function fetchDinos(): Promise<Dino[]> {
  const res = await fetch("/api/dinos");
  if (!res.ok) {
    throw new Error("Falha ao carregar dinossauros.");
  }
  const data = (await res.json()) as { dinos: Dino[] };
  return data.dinos;
}

export async function createDino(input: NewDinoInput): Promise<Dino> {
  const res = await fetch("/api/dinos", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  const data = await res.json();
  if (!res.ok) {
    const errors: string[] = data.errors ?? [data.error ?? "Erro desconhecido."];
    throw new Error(errors.join(" "));
  }
  return (data as { dino: Dino }).dino;
}
