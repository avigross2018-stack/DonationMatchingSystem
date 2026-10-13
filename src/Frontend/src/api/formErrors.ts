import type { FieldValues, Path, UseFormSetError } from 'react-hook-form';
import type { AppError } from './types';

export function applyServerFieldErrors<T extends FieldValues>(error: AppError, setError: UseFormSetError<T>) {
  for (const [serverName, messages] of Object.entries(error.fieldErrors)) {
    if (!messages.length) continue;
    const fieldName = `${serverName.charAt(0).toLowerCase()}${serverName.slice(1)}` as Path<T>;
    setError(fieldName, { type: 'server', message: messages.join(' ') });
  }
}
