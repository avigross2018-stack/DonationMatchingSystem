import { z } from 'zod';
export const loginSchema = z.object({ phone: z.string().min(1, 'יש להזין מספר טלפון').max(50), password: z.string().min(8, 'הסיסמה חייבת להכיל לפחות 8 תווים') });
export const donorRegistrationSchema = z.object({ name: z.string().min(1, 'יש להזין שם').max(50, 'עד 50 תווים'), phone: z.string().min(1, 'יש להזין טלפון').max(50), password: z.string().min(8, 'לפחות 8 תווים'), confirmPassword: z.string() }).refine((data) => data.password === data.confirmPassword, { path: ['confirmPassword'], message: 'הסיסמאות אינן זהות' });
const twoDecimals = (value: number) => Math.abs(value * 100 - Math.round(value * 100)) < Number.EPSILON * 100;
export const recipientRegistrationSchema = donorRegistrationSchema.and(z.object({
  populationTypeId: z.number().int().positive('יש לבחור סוג אוכלוסייה'),
  incomeAmount: z.number().min(0, 'הכנסה אינה יכולה להיות שלילית').max(99999999.99).refine(twoDecimals, 'עד שתי ספרות אחרי הנקודה'),
  amountOfChildren: z.number().int('יש להזין מספר שלם').min(0, 'מספר הילדים אינו יכול להיות שלילי'),
  requestReason: z.string().max(250, 'עד 250 תווים').optional(),
}));
export const passwordSchema = z.object({ currentPassword: z.string().min(1, 'יש להזין סיסמה נוכחית'), newPassword: z.string().min(8, 'לפחות 8 תווים'), confirmPassword: z.string() }).refine((data) => data.newPassword === data.confirmPassword, { path: ['confirmPassword'], message: 'הסיסמאות אינן זהות' });
export type LoginForm = z.infer<typeof loginSchema>;
export type DonorRegistrationForm = z.infer<typeof donorRegistrationSchema>;
export type RecipientRegistrationForm = z.infer<typeof recipientRegistrationSchema>;
export type PasswordForm = z.infer<typeof passwordSchema>;
