import { InjectionToken } from '@angular/core';
import { EsSearchRequest, EsSearchResponse } from './types';
import { Movie } from '../models/movie';
import { Screening } from '../models/screening';

export interface SearchService {
    searchMovies(req: EsSearchRequest): Promise<EsSearchResponse<Movie>>;
}

export const SEARCH_SERVICE = new InjectionToken<SearchService>('SEARCH_SERVICE');
