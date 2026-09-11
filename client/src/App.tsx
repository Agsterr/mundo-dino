import { FormEvent, useEffect, useMemo, useState } from "react";
import { createDino, fetchDinos } from "./api";
import { Diet, Dino, NewDinoInput } from "./types";

const DIET_LABELS: Record<Diet, string> = {
  herbivoro: "Herbívoro",
  carnivoro: "Carnívoro",
  onivoro: "Onívoro",
};

const DIET_EMOJI: Record<Diet, string> = {
  herbivoro: "🌿",
  carnivoro: "🥩",
  onivoro: "🍽️",
};

const emptyForm: NewDinoInput = {
  name: "",
  species: "",
  period: "Cretáceo",
  diet: "herbivoro",
  lengthMeters: 5,
  emoji: "🦕",
  funFact: "",
};

export function App() {
  const [dinos, setDinos] = useState<Dino[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<NewDinoInput>(emptyForm);
  const [formError, setFormError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [filter, setFilter] = useState<Diet | "todos">("todos");

  useEffect(() => {
    fetchDinos()
      .then(setDinos)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, []);

  const visibleDinos = useMemo(
    () => (filter === "todos" ? dinos : dinos.filter((d) => d.diet === filter)),
    [dinos, filter],
  );

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setFormError(null);
    setSubmitting(true);
    try {
      const created = await createDino({
        ...form,
        lengthMeters: Number(form.lengthMeters),
      });
      setDinos((prev) => [...prev, created]);
      setForm(emptyForm);
    } catch (e) {
      setFormError((e as Error).message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="page">
      <header className="hero">
        <h1>
          <span className="hero-emoji">🦕</span> Mundo Dino
        </h1>
        <p>Explore e cadastre os habitantes do parque jurássico.</p>
      </header>

      <main className="layout">
        <section className="panel">
          <div className="panel-head">
            <h2>Habitantes ({visibleDinos.length})</h2>
            <div className="filters" role="group" aria-label="Filtrar por dieta">
              {(["todos", "herbivoro", "carnivoro", "onivoro"] as const).map((f) => (
                <button
                  key={f}
                  type="button"
                  className={filter === f ? "chip chip-active" : "chip"}
                  onClick={() => setFilter(f)}
                >
                  {f === "todos" ? "Todos" : DIET_LABELS[f]}
                </button>
              ))}
            </div>
          </div>

          {loading && <p className="muted">Carregando dinossauros...</p>}
          {error && <p className="alert">⚠️ {error}</p>}

          <ul className="dino-grid">
            {visibleDinos.map((dino) => (
              <li key={dino.id} className="dino-card" data-testid="dino-card">
                <div className="dino-emoji">{dino.emoji}</div>
                <div className="dino-body">
                  <h3>{dino.name}</h3>
                  <p className="species">{dino.species}</p>
                  <div className="tags">
                    <span className="tag">{dino.period}</span>
                    <span className="tag">
                      {DIET_EMOJI[dino.diet]} {DIET_LABELS[dino.diet]}
                    </span>
                    <span className="tag">📏 {dino.lengthMeters} m</span>
                  </div>
                  <p className="fact">{dino.funFact}</p>
                </div>
              </li>
            ))}
          </ul>
          {!loading && visibleDinos.length === 0 && (
            <p className="muted">Nenhum dinossauro nessa categoria ainda.</p>
          )}
        </section>

        <section className="panel form-panel">
          <h2>Adicionar dinossauro</h2>
          <form onSubmit={handleSubmit} className="form">
            <label htmlFor="dino-name">
              Nome
              <input
                id="dino-name"
                name="name"
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
                placeholder="Ex: Denver"
                required
              />
            </label>
            <label htmlFor="dino-species">
              Espécie
              <input
                id="dino-species"
                name="species"
                value={form.species}
                onChange={(e) => setForm({ ...form, species: e.target.value })}
                placeholder="Ex: Corythosaurus"
                required
              />
            </label>
            <label htmlFor="dino-period">
              Período
              <input
                id="dino-period"
                name="period"
                value={form.period}
                onChange={(e) => setForm({ ...form, period: e.target.value })}
                placeholder="Ex: Cretáceo"
                required
              />
            </label>
            <div className="row">
              <label htmlFor="dino-diet">
                Dieta
                <select
                  id="dino-diet"
                  name="diet"
                  value={form.diet}
                  onChange={(e) => setForm({ ...form, diet: e.target.value as Diet })}
                >
                  <option value="herbivoro">Herbívoro</option>
                  <option value="carnivoro">Carnívoro</option>
                  <option value="onivoro">Onívoro</option>
                </select>
              </label>
              <label htmlFor="dino-length">
                Tamanho (m)
                <input
                  id="dino-length"
                  name="lengthMeters"
                  type="number"
                  min={0.1}
                  step={0.1}
                  value={form.lengthMeters}
                  onChange={(e) =>
                    setForm({ ...form, lengthMeters: Number(e.target.value) })
                  }
                  required
                />
              </label>
              <label className="emoji-field" htmlFor="dino-emoji">
                Emoji
                <input
                  id="dino-emoji"
                  name="emoji"
                  value={form.emoji}
                  onChange={(e) => setForm({ ...form, emoji: e.target.value })}
                  maxLength={4}
                />
              </label>
            </div>
            <label htmlFor="dino-fact">
              Curiosidade
              <textarea
                id="dino-fact"
                name="funFact"
                value={form.funFact}
                onChange={(e) => setForm({ ...form, funFact: e.target.value })}
                placeholder="Conte algo interessante sobre esse dino"
                rows={3}
              />
            </label>
            {formError && <p className="alert">⚠️ {formError}</p>}
            <button type="submit" className="submit" disabled={submitting}>
              {submitting ? "Salvando..." : "🦖 Adicionar ao parque"}
            </button>
          </form>
        </section>
      </main>
    </div>
  );
}
