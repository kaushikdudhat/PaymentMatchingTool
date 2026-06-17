import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { MatchingService } from '../services/matching.service';
import { MatchResult } from '../models/match-result.model';
import { MatchSummary } from '../models/match-summary.model';

@Component({
  selector: 'app-payments-matching',
  standalone: true,
  imports: [
    CommonModule,
    DecimalPipe,
    MatTableModule,
    MatButtonModule,
    MatCardModule,
    MatSelectModule,
    MatFormFieldModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDividerModule,
    MatTooltipModule
  ],
  templateUrl: './payments-matching.component.html',
  styleUrl: './payments-matching.component.scss'
})
export class PaymentsMatchingComponent implements OnInit {

  summary: MatchSummary | null = null;
  results: MatchResult[] = [];
  selectedFilter = 'all';
  isRunning  = false;
  isLoading  = false;
  submitted  = false;

  systemFile: File | null   = null;
  providerFile: File | null = null;

  readonly displayedColumns = [
    'orderId', 'systemAmount', 'providerAmount', 'currency',
    'status', 'isResolved', 'resolutionSide', 'actions'
  ];

  readonly filterOptions = [
    { value: 'all',        label: 'All'        },
    { value: 'resolved',   label: 'Resolved'   },
    { value: 'unresolved', label: 'Unresolved' }
  ];

  constructor(
    private matchingService: MatchingService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadSummary();
    this.loadResults();
  }

  onSystemFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.systemFile = input.files?.[0] ?? null;
  }

  onProviderFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.providerFile = input.files?.[0] ?? null;
  }

  runMatch(): void {
    this.submitted = true;

    if (!this.systemFile || !this.providerFile) {
      this.snackBar.open('Both CSV files are required.', 'Close', { duration: 3000 });
      return;
    }

    this.isRunning = true;

    this.matchingService.runMatching(this.systemFile, this.providerFile).subscribe({
      next: summary => {
        this.summary   = summary;
        this.submitted = false;
        this.isRunning = false;
        this.selectedFilter = 'all';
        this.loadResults();
        this.snackBar.open('Matching completed successfully!', 'Close',
          { duration: 3000, panelClass: ['success-snack'] });
      },
      error: err => {
        this.isRunning = false;
        const msg = err.error?.error ?? 'Failed to run matching. Check the API is running.';
        this.snackBar.open(msg, 'Close', { duration: 6000, panelClass: ['error-snack'] });
      }
    });
  }

  onFilterChange(value: string): void {
    this.selectedFilter = value;
    this.loadResults();
  }

  loadResults(): void {
    this.isLoading = true;
    this.matchingService.getResults(this.selectedFilter).subscribe({
      next: results => {
        this.results   = results;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  loadSummary(): void {
    this.matchingService.getSummary().subscribe({
      next: summary => { this.summary = summary; },
      error: () => {}
    });
  }

  resolve(result: MatchResult, side: 'SYSTEM' | 'PROVIDER'): void {
    this.matchingService.resolve(result.id, side).subscribe({
      next: updated => {
        const idx = this.results.findIndex(r => r.id === updated.id);
        if (idx !== -1) {
          this.results = [
            ...this.results.slice(0, idx),
            updated,
            ...this.results.slice(idx + 1)
          ];
        }
        this.loadSummary();
        this.snackBar.open(`Resolved: accepted ${side} amount.`, 'Close',
          { duration: 3000, panelClass: ['success-snack'] });
      },
      error: err => {
        const msg = err.error?.error ?? 'Failed to resolve record.';
        this.snackBar.open(msg, 'Close', { duration: 4000, panelClass: ['error-snack'] });
      }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'MATCHED':        'badge-matched',
      'AMOUNTMISMATCH': 'badge-mismatch',
      'ONLYSYSTEM':     'badge-system',
      'ONLYPROVIDER':   'badge-provider'
    };
    return map[status] ?? '';
  }

  canResolve(result: MatchResult): boolean {
    return !result.isResolved && result.status !== 'MATCHED';
  }
}
