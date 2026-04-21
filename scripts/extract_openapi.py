"""Extract the embedded OpenAPI spec from the easyVerein Swagger UI HTML page.

The Swagger UI page at /api/v2.0/documentation embeds the full OpenAPI 3.0
spec as a JavaScript literal: ``var spec = {...};``. This script parses the
HTML, locates that literal, extracts the balanced JSON object, and writes it
as JSON and YAML.

Run from repo root:
    python scripts/extract_openapi.py <path-to-html> <output-dir>

Produces:
    <output-dir>/easyverein-v2.0.json
    <output-dir>/easyverein-v2.0.yaml
"""
from __future__ import annotations

import json
import sys
from pathlib import Path

import yaml


def extract_spec_literal(html: str) -> str:
    marker = "var spec = "
    start = html.find(marker)
    if start == -1:
        raise RuntimeError("Marker 'var spec = ' not found in HTML.")
    start += len(marker)

    depth = 0
    in_str = False
    escape = False
    i = start
    n = len(html)
    if html[i] != "{":
        raise RuntimeError(f"Expected '{{' after marker, got {html[i]!r}.")
    while i < n:
        ch = html[i]
        if in_str:
            if escape:
                escape = False
            elif ch == "\\":
                escape = True
            elif ch == '"':
                in_str = False
        else:
            if ch == '"':
                in_str = True
            elif ch == "{":
                depth += 1
            elif ch == "}":
                depth -= 1
                if depth == 0:
                    return html[start : i + 1]
        i += 1
    raise RuntimeError("Unbalanced braces while scanning spec literal.")


def main(argv: list[str]) -> int:
    if len(argv) != 3:
        print(__doc__)
        return 2

    html_path = Path(argv[1])
    out_dir = Path(argv[2])
    out_dir.mkdir(parents=True, exist_ok=True)

    html = html_path.read_text(encoding="utf-8")
    literal = extract_spec_literal(html)
    spec = json.loads(literal)

    json_path = out_dir / "easyverein-v2.0.json"
    yaml_path = out_dir / "easyverein-v2.0.yaml"

    json_path.write_text(
        json.dumps(spec, indent=2, ensure_ascii=False, sort_keys=False),
        encoding="utf-8",
    )
    with yaml_path.open("w", encoding="utf-8") as f:
        yaml.safe_dump(spec, f, allow_unicode=True, sort_keys=False, width=120)

    print(f"openapi: {spec.get('openapi')}")
    print(f"title:   {spec.get('info', {}).get('title')}")
    print(f"version: {spec.get('info', {}).get('version')}")
    print(f"paths:   {len(spec.get('paths', {}))}")
    print(f"schemas: {len(spec.get('components', {}).get('schemas', {}))}")
    print(f"wrote:   {json_path}  ({json_path.stat().st_size:,} bytes)")
    print(f"wrote:   {yaml_path}  ({yaml_path.stat().st_size:,} bytes)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))
