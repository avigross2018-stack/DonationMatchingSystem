import { Alert, Button, FormControl, FormHelperText, InputLabel, MenuItem, Select, Stack, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useId } from 'react';
import { queryKeys } from '../api/queryKeys';
import { populationTypesApi } from '../api/services';

export function useActivePopulationTypes() {
  return useQuery({ queryKey: queryKeys.activePopulationTypes, queryFn: populationTypesApi.listActive });
}

export function PopulationTypeSelect({ value, onChange, disabled, error, helperText, label = 'סוג אוכלוסייה' }: { value: number | ''; onChange: (value: number) => void; disabled?: boolean; error?: boolean; helperText?: string; label?: string }) {
  const query = useActivePopulationTypes();
  const labelId = useId();
  if (query.isError) return <Alert severity="error" action={<Button color="inherit" onClick={() => query.refetch()}>נסו שוב</Button>}>לא ניתן לטעון את סוגי האוכלוסייה.</Alert>;
  if (!query.isLoading && !query.data?.length) return <Alert severity="warning">אין כרגע סוגי אוכלוסייה פעילים. יש לפנות למנהל המערכת.</Alert>;
  return <Stack spacing={.75}>
    <FormControl fullWidth error={error} disabled={disabled || query.isLoading || !query.data?.length}>
      <InputLabel id={labelId}>{query.isLoading ? 'טוען סוגי אוכלוסייה…' : label}</InputLabel>
      <Select labelId={labelId} inputProps={{ 'aria-label': label }} label={query.isLoading ? 'טוען סוגי אוכלוסייה…' : label} value={value} onChange={(event) => onChange(Number(event.target.value))}>
        {query.data?.map((type) => <MenuItem key={type.id} value={type.id}><Stack><Typography>{type.name}</Typography>{type.description && <Typography variant="caption" color="text.secondary">{type.description}</Typography>}</Stack></MenuItem>)}
      </Select>
      {helperText && <FormHelperText>{helperText}</FormHelperText>}
    </FormControl>
  </Stack>;
}
