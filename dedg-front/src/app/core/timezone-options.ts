export interface TimezoneOption {
  value: string;
  label: string;
  offset: number; // minutes from UTC, used for sorting
}

function utcOffsetMinutes(tz: string): number {
  const parts = new Intl.DateTimeFormat('en', {
    timeZone: tz,
    timeZoneName: 'shortOffset',
  }).formatToParts(new Date());
  const raw = parts.find((p) => p.type === 'timeZoneName')?.value ?? '';
  const m = raw.match(/GMT([+-])(\d+)(?::(\d+))?/);
  if (!m) return 0;
  const sign = m[1] === '+' ? 1 : -1;
  return sign * (parseInt(m[2], 10) * 60 + parseInt(m[3] ?? '0', 10));
}

function offsetLabel(minutes: number): string {
  const sign = minutes >= 0 ? '+' : '-';
  const abs = Math.abs(minutes);
  const h = String(Math.floor(abs / 60)).padStart(2, '0');
  const min = String(abs % 60).padStart(2, '0');
  return `UTC${sign}${h}:${min}`;
}

const FALLBACK: string[] = [
  'America/Sao_Paulo', 'America/Manaus', 'America/Fortaleza', 'America/Recife',
  'America/Belem', 'America/Boa_Vista', 'America/Porto_Velho', 'America/Noronha',
  'America/New_York', 'America/Chicago', 'America/Denver', 'America/Los_Angeles',
  'America/Bogota', 'America/Lima', 'America/Buenos_Aires', 'America/Santiago',
  'America/Mexico_City', 'America/Montevideo', 'America/Caracas',
  'Europe/Lisbon', 'Europe/London', 'Europe/Paris', 'Europe/Berlin',
  'Europe/Madrid', 'Europe/Rome', 'Europe/Amsterdam', 'Europe/Warsaw',
  'Europe/Helsinki', 'Europe/Istanbul', 'Europe/Moscow',
  'Africa/Johannesburg', 'Africa/Lagos', 'Africa/Nairobi',
  'Asia/Dubai', 'Asia/Karachi', 'Asia/Kolkata', 'Asia/Dhaka',
  'Asia/Bangkok', 'Asia/Singapore', 'Asia/Shanghai', 'Asia/Tokyo',
  'Asia/Seoul', 'Asia/Jakarta',
  'Australia/Sydney', 'Australia/Perth', 'Australia/Brisbane',
  'Pacific/Auckland', 'Pacific/Honolulu', 'Pacific/Fiji',
  'UTC',
];

let _cache: TimezoneOption[] | null = null;

export function buildTimezoneOptions(): TimezoneOption[] {
  if (_cache) return _cache;

  let zones: string[];
  try {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    zones = (Intl as any).supportedValuesOf('timeZone') as string[];
  } catch {
    zones = FALLBACK;
  }

  _cache = zones
    .map((tz) => {
      const offset = utcOffsetMinutes(tz);
      const city = tz.split('/').pop()!.replace(/_/g, ' ');
      return { value: tz, label: `(${offsetLabel(offset)}) ${city}`, offset };
    })
    .sort((a, b) => a.offset !== b.offset ? a.offset - b.offset : a.value.localeCompare(b.value));

  return _cache;
}
