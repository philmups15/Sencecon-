export const font = {
  display: "'Manrope', -apple-system, sans-serif",
  body: "'Source Sans 3', -apple-system, sans-serif",
  mono: "'SF Mono', Consolas, monospace",
}

export const color = {
  primary: '#1F6E72',
  primaryTint: '#E4F0EF',
  deep: '#12484B',
  mid: '#2E9E8F',
  accent: '#3FD07A',
  accentTint: '#E1F9EA',
  spark: '#C9F24D',

  ink: '#12201F',
  slate: '#52685F',
  slateSoft: '#78908A',
  mist: '#F4F8F7',
  mist2: '#E9F1EF',
  line: '#D7E4E1',
  white: '#FFFFFF',

  success: '#1C8A4E',
  successBg: '#E3F8EC',
  info: '#1F6E72',
  infoBg: '#E4F0EF',
  warning: '#8A5A16',
  warningBg: '#FBF0DC',
  error: '#A6362E',
  errorBg: '#FBE7E5',
  inactive: '#52685F',
  inactiveBg: '#EAEFED',
}

export type ChipTone = 'primary' | 'success' | 'warning' | 'error' | 'accent' | 'slate'

export const toneMap: Record<ChipTone, { bg: string; fg: string; dot: string }> = {
  primary: { bg: color.infoBg, fg: color.deep, dot: color.primary },
  success: { bg: color.successBg, fg: color.success, dot: color.success },
  warning: { bg: color.warningBg, fg: color.warning, dot: color.warning },
  error: { bg: color.errorBg, fg: color.error, dot: color.error },
  accent: { bg: color.accentTint, fg: color.deep, dot: color.accent },
  slate: { bg: color.inactiveBg, fg: color.inactive, dot: color.inactive },
}
