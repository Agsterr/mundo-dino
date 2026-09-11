#!/usr/bin/env node
const fs = require('fs');
const path = require('path');

const root = path.join(__dirname, '..');
const ignore = new Set(['.git', 'node_modules', 'Library', 'Temp', 'Logs', 'obj']);

function walk(dir, prefix = '') {
  const entries = fs.readdirSync(dir, { withFileTypes: true })
    .filter(e => !ignore.has(e.name))
    .sort((a, b) => a.name.localeCompare(b.name));

  entries.forEach((entry, i) => {
    const isLast = i === entries.length - 1;
    const connector = isLast ? '└── ' : '├── ';
    console.log(prefix + connector + entry.name);
    if (entry.isDirectory()) {
      walk(path.join(dir, entry.name), prefix + (isLast ? '    ' : '│   '));
    }
  });
}

console.log('open-world-dino-survival/');
walk(root);
