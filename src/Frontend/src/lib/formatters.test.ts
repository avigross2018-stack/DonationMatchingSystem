import { describe, expect, it } from 'vitest';
import { formatCurrency, statusLabels } from './formatters';
describe('formatters', () => {
  it('formats ILS amounts for Hebrew users', () => { expect(formatCurrency(1234)).toContain('1,234'); expect(formatCurrency(1234)).toContain('₪'); });
  it('maps known statuses to Hebrew', () => { expect(statusLabels.PENDING).toBe('ממתין לאישור'); });
});
