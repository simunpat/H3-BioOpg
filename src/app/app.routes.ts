import { Routes } from '@angular/router';
import { ScreeningsListComponent } from './features/screenings/screenings-list.component';
import { BookingsListComponent } from './features/bookings/bookings-list.component';
import { AuditoriumsListComponent } from './features/auditoriums/auditoriums-list.component';
import { UsersListComponent } from './features/users/users-list.component';
import { LoginComponent } from './auth/login.component';
import { RegisterComponent } from './auth/register.component';
import { authGuard } from './core/auth/auth.guard';
import { adminGuard } from './core/auth/role.guard';
import { alreadyAuthGuard } from './core/auth/already-auth.guard';
import { BookingScreenComponent } from './features/bookings/booking-screen.component';
import { MovieDetailComponent } from './features/movies/movie-detail.component';
import { MoviesBrowseComponent } from './features/movies/movies-browse.component';
import { AdminMoviesComponent } from './features/movies/admin-movies.component';
import { AdminMovieFormComponent } from './features/movies/admin-movie-form.component';
import { UserFormComponent } from './features/users/user-form.component';
import { HackSqlComponent } from './features/hack-sql/hack-sql.component';
import { MyBookingsComponent } from './features/bookings/my-bookings.component';

export const routes: Routes = [
    { path: '', component: MoviesBrowseComponent },
    { path: 'login', component: LoginComponent, canActivate: [alreadyAuthGuard] },
    { path: 'register', component: RegisterComponent, canActivate: [alreadyAuthGuard] },
    { path: 'movies/:id', component: MovieDetailComponent },
    { path: 'screenings/:id/book', component: BookingScreenComponent },
    { path: 'my/bookings', component: MyBookingsComponent, canActivate: [authGuard] },
    {
        path: 'bookings/:id/confirmation',
        loadComponent: () =>
            import('./features/bookings/booking-confirmation.component').then(
                (m) => m.BookingConfirmationComponent
            ),
    },
    { path: 'hack/sql', component: HackSqlComponent },
    {
        path: 'admin',
        canActivate: [authGuard, adminGuard],
        children: [
            { path: '', pathMatch: 'full', redirectTo: 'movies' },
            { path: 'movies', component: AdminMoviesComponent },
            { path: 'movies/new', component: AdminMovieFormComponent },
            { path: 'movies/:id/edit', component: AdminMovieFormComponent },
            { path: 'bookings', component: BookingsListComponent },
            { path: 'auditoriums', component: AuditoriumsListComponent },
            { path: 'screenings', component: ScreeningsListComponent },
            { path: 'users', component: UsersListComponent },
            { path: 'users/new', component: UserFormComponent },
            { path: 'users/:id/edit', component: UserFormComponent },
        ],
    },
    { path: '**', redirectTo: '' },
];
