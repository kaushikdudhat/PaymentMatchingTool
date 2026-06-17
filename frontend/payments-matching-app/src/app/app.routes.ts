import { Routes } from '@angular/router';
import { PaymentsMatchingComponent } from './features/payments-matching/pages/payments-matching.component';

export const routes: Routes = [
  { path: '', component: PaymentsMatchingComponent },
  { path: '**', redirectTo: '' }
];
