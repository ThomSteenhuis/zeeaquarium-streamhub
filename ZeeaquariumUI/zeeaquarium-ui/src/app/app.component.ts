import { Component, OnDestroy, OnInit } from '@angular/core';
import { combineLatest, interval, merge, Observable, of, Subscription } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { getTemperatureValueState, ValueState } from './shared/constants';

import { SignalRService } from './shared/signalr.service';

export interface IMeasurement {
  name: string;
  value: string;
  unit: string;
  timeStamp: number;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'zeeaquarium-ui';

  public temperature$: Observable<IMeasurement> = of({
    name: 'Temperatuur', 
    value: '',  
    unit: '\u00B0C', 
    timeStamp: 0
  });
  public currentTime$: Observable<number>;

  public subscriptions = new Subscription();

  constructor(public signalRService: SignalRService) {
    this.currentTime$ = merge(of(''), interval(1000)).pipe(
      map(() => new Date().valueOf())
    );
  }

  public ngOnInit(): void {
    this.subscriptions.add(merge(of(''), interval(5000)).pipe(
      tap(() => this.signalRService.startConnection())
    ).subscribe());

    this.temperature$ = this.signalRService.getTemperature();
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public isConnected(): boolean {
    return this.signalRService.isConnected();
  }

  public isConnecting(): boolean {
    return this.signalRService.isConnecting() || this.signalRService.isReconnecting();
  }

  public isDisconnected(): boolean {
    return this.signalRService.isDisconnected();
  }

  public isUnknown(): boolean {
    return !this.isConnected() && !this.isConnecting() && !this.isDisconnected();
  }

  public isMeasurement(measurement$: Observable<IMeasurement>): Observable<boolean> {
    return measurement$.pipe(
      map((measurement: IMeasurement) => measurement.value != null && measurement.value !== '')
    )
  }

  public getName(measurement$: Observable<IMeasurement>): Observable<string> {
    return measurement$.pipe(
      map((measurement: IMeasurement) => measurement.name)
    );
  }

  public getValue(measurement$: Observable<IMeasurement>): Observable<string> {
    return measurement$.pipe(
      map((measurement: IMeasurement) => measurement.value)
    );
  }

  public getUnit(measurement$: Observable<IMeasurement>): Observable<string> {
    return measurement$.pipe(
      map((measurement: IMeasurement) => measurement.unit)
    );
  }

  public getSecondsAgo(measurement$: Observable<IMeasurement>): Observable<number> {
    return combineLatest([this.currentTime$, measurement$]).pipe(
      map(([currentTime, measurement]) => Math.max(0, Math.round((currentTime - measurement.timeStamp) / 1000)))
    );
  }

  public moreThanAMinuteAgo(measurement$: Observable<IMeasurement>): Observable<boolean> {
    return this.getSecondsAgo(measurement$).pipe(
      map((secondsAgo: number) => secondsAgo > 60)
    );
  }

  public getValueState(measurement$: Observable<IMeasurement>): Observable<string> {
    return measurement$.pipe(
      map((measurement: IMeasurement) => {
        const valueState = getTemperatureValueState(measurement.value);
        if (valueState === ValueState.Good) {
          return 'good'
        }
        if (valueState === ValueState.Questionable) {
          return 'questionable'
        }
        if (valueState === ValueState.Dangerous) {
          return 'dangerous'
        }

        return 'unknown';
      })
    );
  }
}
