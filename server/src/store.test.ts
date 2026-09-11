import assert from "node:assert/strict";
import { test } from "node:test";
import { DinoStore, ValidationError, validateNewDino } from "./store.js";

test("list returns seeded dinos", () => {
  const store = new DinoStore();
  assert.ok(store.list().length >= 5);
});

test("add creates a dino with a generated id", () => {
  const store = new DinoStore();
  const before = store.list().length;
  const dino = store.add({
    name: "Denver",
    species: "Corythosaurus",
    period: "Cretaceo",
    diet: "herbivoro",
    lengthMeters: 9,
  });
  assert.ok(dino.id);
  assert.equal(dino.name, "Denver");
  assert.equal(store.list().length, before + 1);
});

test("add rejects invalid input", () => {
  const store = new DinoStore();
  assert.throws(
    () =>
      store.add({
        name: "",
        species: "",
        period: "",
        // @ts-expect-error testing invalid diet
        diet: "plastico",
        lengthMeters: -3,
      }),
    ValidationError,
  );
});

test("validateNewDino reports missing fields", () => {
  const errors = validateNewDino({});
  assert.ok(errors.length >= 4);
});
