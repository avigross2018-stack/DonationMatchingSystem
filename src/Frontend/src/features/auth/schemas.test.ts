import { describe, expect, it } from 'vitest';
import { recipientRegistrationSchema } from './schemas';

const valid = { name: 'ישראל ישראלי', phone: '0501234567', password: 'password1', confirmPassword: 'password1', populationTypeId: 12, incomeAmount: 1234.56, amountOfChildren: 2, requestReason: 'בקשת סיוע' };

describe('recipientRegistrationSchema', () => {
  it('accepts a server-provided positive population type id', () => {
    expect(recipientRegistrationSchema.safeParse(valid).success).toBe(true);
  });

  it('rejects missing population selection and invalid eligibility inputs', () => {
    expect(recipientRegistrationSchema.safeParse({ ...valid, populationTypeId: 0 }).success).toBe(false);
    expect(recipientRegistrationSchema.safeParse({ ...valid, incomeAmount: 1.234 }).success).toBe(false);
    expect(recipientRegistrationSchema.safeParse({ ...valid, amountOfChildren: -1 }).success).toBe(false);
  });
});
