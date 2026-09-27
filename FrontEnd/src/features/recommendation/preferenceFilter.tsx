import * as React from 'react';
import {
    Accordion,
    AccordionSummary,
    AccordionDetails,
    Grid,
    Typography,
    Slider,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    TextField,
    FormControlLabel,
    Checkbox,
    Stack,
    Button,
    Box
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import FilterListIcon from '@mui/icons-material/FilterList';

export interface FilterState {
    ageRange: number[];
    gender: string;
    city: string;
    commonSongOnly: boolean;
}

interface PreferenceFiltersProps {
    initialPreferences?: FilterState;
    onApplyFilters?: (filters: FilterState) => void;
}

export function PreferenceFilters({ initialPreferences, onApplyFilters }: PreferenceFiltersProps) {
    const [ageRange, setAgeRange] = React.useState<number[]>([18, 99]);
    const [gender, setGender] = React.useState<string>('all');
    const [commonSongOnly, setCommonSongOnly] = React.useState<boolean>(false);
    const [city, setCity] = React.useState<string>('');

    // Kezdeti értékek beállítása, ha megérkeznek a backendről
    React.useEffect(() => {
        if (initialPreferences) {
            setAgeRange(initialPreferences.ageRange);
            setGender(initialPreferences.gender);
            setCity(initialPreferences.city);
            setCommonSongOnly(initialPreferences.commonSongOnly);
        }
    }, [initialPreferences]);

    const handleResetFilters = () => {
        const defaultState: FilterState = {
            ageRange: [18, 99],
            gender: 'all',
            commonSongOnly: false,
            city: ''
        };
        setAgeRange(defaultState.ageRange);
        setGender(defaultState.gender);
        setCommonSongOnly(defaultState.commonSongOnly);
        setCity(defaultState.city);

        if (onApplyFilters) {
            onApplyFilters(defaultState);
        }
    };

    const handleApply = () => {
        if (onApplyFilters) {
            onApplyFilters({ ageRange, gender, city, commonSongOnly });
        }
    };

    return (
        <Accordion
            sx={{
                mb: 3,
                borderRadius: '16px !important',
                boxShadow: '0 4px 12px rgba(0,0,0,0.05)',
                '&:before': { display: 'none' },
                overflow: 'hidden',
                border: '1px solid #eee'
            }}
        >
            <AccordionSummary
                expandIcon={<ExpandMoreIcon sx={{ color: '#FF007A' }} />}
                sx={{
                    backgroundColor: '#fff',
                    px: 3,
                    '& .MuiAccordionSummary-content': { alignItems: 'center', gap: 1 }
                }}
            >
                <FilterListIcon sx={{ color: '#FF007A' }} />
                <Typography variant="h6" sx={{ fontWeight: 600, fontSize: '1.1rem' }}>
                    Preference Settings
                </Typography>
            </AccordionSummary>

            <AccordionDetails sx={{ p: 3, bgcolor: '#fafafa' }}>
                <Grid container spacing={3} alignItems="center">
                    {/* Age Slider */}
                    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                        <Box
                            sx={{
                                border: '1px solid rgba(0, 0, 0, 0.23)',
                                borderRadius: '10px',
                                px: 2,
                                height: 40,
                                display: 'flex',
                                alignItems: 'center',
                                position: 'relative',
                                bgcolor: '#fff',
                                '&:hover': { borderColor: 'rgba(0, 0, 0, 0.87)' },
                                '&:focus-within': {
                                    borderColor: '#FF007A',
                                    borderWidth: '2px',
                                    px: '15px'
                                }
                            }}
                        >
                            <Typography
                                variant="caption"
                                sx={{
                                    position: 'absolute',
                                    top: -10,
                                    left: 10,
                                    bgcolor: '#fafafa',
                                    px: 0.5,
                                    color: 'text.secondary',
                                    fontSize: '0.75rem',
                                    fontWeight: 500
                                }}
                            >
                                Age: {ageRange[0]} - {ageRange[1]} years old
                            </Typography>
                            <Slider
                                value={ageRange}
                                onChange={(_: Event, newValue: number | number[]) => setAgeRange(newValue as number[])}
                                valueLabelDisplay="auto"
                                min={18}
                                max={99}
                                size="small"
                                sx={{
                                    color: '#FF007A',
                                    p: 0,
                                    '& .MuiSlider-thumb': { backgroundColor: '#FF007A' },
                                }}
                            />
                        </Box>
                    </Grid>

                    {/* Gender Select */}
                    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                        <FormControl fullWidth size="small">
                            <InputLabel>Gender</InputLabel>
                            <Select
                                value={gender}
                                label="Gender"
                                onChange={(e) => setGender((e.target as HTMLInputElement).value || 'all')}
                                sx={{ height: 40, bgcolor: '#fff' }}
                            >
                                <MenuItem value="all">Any</MenuItem>
                                <MenuItem value="female">Female</MenuItem>
                                <MenuItem value="male">Male</MenuItem>
                                <MenuItem value="other">Other</MenuItem>
                            </Select>
                        </FormControl>
                    </Grid>

                    {/* City TextField */}
                    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                        <TextField
                            fullWidth
                            size="small"
                            label="City"
                            placeholder="e.g. London"
                            value={city}
                            onChange={(e) => setCity(e.currentTarget.value)}
                            sx={{
                                bgcolor: '#fff',
                                '& .MuiInputBase-root': { height: 40 }
                            }}
                        />
                    </Grid>

                    {/* Common Favorite Song Checkbox */}
                    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                        <Box
                            sx={{
                                border: '1px solid rgba(0, 0, 0, 0.23)',
                                borderRadius: '10px',
                                px: 1.5,
                                height: 40,
                                display: 'flex',
                                alignItems: 'center',
                                bgcolor: '#fff',
                                '&:hover': { borderColor: 'rgba(0, 0, 0, 0.87)' }
                            }}
                        >
                            <FormControlLabel
                                control={
                                    <Checkbox
                                        checked={commonSongOnly}
                                        onChange={(e) => setCommonSongOnly(e.currentTarget.checked)}
                                        size="small"
                                        sx={{
                                            color: '#FF007A',
                                            '&.Mui-checked': { color: '#FF007A' },
                                        }}
                                    />
                                }
                                label={
                                    <Typography variant="body2" sx={{ fontSize: '0.875rem' }}>
                                        Common favorite song
                                    </Typography>
                                }
                                sx={{ m: 0, width: '100%' }}
                            />
                        </Box>
                    </Grid>
                </Grid>

                {/* Action Buttons */}
                <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ mt: 3 }}>
                    <Button
                        variant="text"
                        onClick={handleResetFilters}
                        sx={{ color: 'text.secondary' }}
                    >
                        Reset Filters
                    </Button>
                    <Button
                        variant="contained"
                        onClick={handleApply}
                        sx={{
                            background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                            color: 'white',
                            fontWeight: 'bold',
                            borderRadius: '20px',
                            px: 3
                        }}
                    >
                        Apply Filters
                    </Button>
                </Stack>
            </AccordionDetails>
        </Accordion>
    );
}