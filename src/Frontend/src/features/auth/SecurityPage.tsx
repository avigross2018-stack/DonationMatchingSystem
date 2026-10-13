import { zodResolver } from '@hookform/resolvers/zod';
import { Alert, Button, Card, CardContent, Stack, TextField } from '@mui/material';
import { useMutation } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { normalizeError } from '../../api/client';
import { authApi } from '../../api/services';
import { PageHeader } from '../../components/PageHeader';
import { passwordSchema, type PasswordForm } from './schemas';

export function SecurityPage() {
  const [message, setMessage] = useState<{ text: string; success: boolean } | null>(null); const { register, handleSubmit, reset, formState: { errors } } = useForm<PasswordForm>({ resolver: zodResolver(passwordSchema) });
  const mutation = useMutation({ mutationFn: authApi.changePassword, onSuccess: () => { reset(); setMessage({ text: 'הסיסמה שונתה בהצלחה.', success: true }); }, onError: (e) => setMessage({ text: normalizeError(e).message, success: false }) });
  return <><PageHeader title="אבטחה וסיסמה" subtitle="מומלץ לבחור סיסמה ייחודית שאינה בשימוש במקום אחר" /><Card><CardContent sx={{ maxWidth: 640 }}>{message && <Alert severity={message.success ? 'success' : 'error'} sx={{ mb: 3 }}>{message.text}</Alert>}<Stack component="form" spacing={2.5} onSubmit={handleSubmit((data) => mutation.mutate({ currentPassword: data.currentPassword, newPassword: data.newPassword }))}><TextField label="סיסמה נוכחית" type="password" autoComplete="current-password" error={Boolean(errors.currentPassword)} helperText={errors.currentPassword?.message} {...register('currentPassword')} /><TextField label="סיסמה חדשה" type="password" autoComplete="new-password" error={Boolean(errors.newPassword)} helperText={errors.newPassword?.message} {...register('newPassword')} /><TextField label="אימות סיסמה חדשה" type="password" autoComplete="new-password" error={Boolean(errors.confirmPassword)} helperText={errors.confirmPassword?.message} {...register('confirmPassword')} /><Button type="submit" variant="contained" disabled={mutation.isPending} sx={{ alignSelf: 'flex-start' }}>{mutation.isPending ? 'מעדכנים…' : 'שינוי סיסמה'}</Button></Stack></CardContent></Card></>;
}
