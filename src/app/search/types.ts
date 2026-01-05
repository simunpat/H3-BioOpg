export interface EsSearchRequest<TQuery = unknown> {
    index: string;
    from?: number;
    size?: number;
    sort?: Array<{ [field: string]: 'asc' | 'desc' }>;
    query: TQuery;
}

export interface EsHit<TSource> {
    _index: string;
    _id: string;
    _score?: number;
    _source: TSource;
    highlight?: { [field: string]: string[] };
}

export interface EsSearchResponse<TSource> {
    hits: {
        total: { value: number };
        hits: Array<EsHit<TSource>>;
    };
}
