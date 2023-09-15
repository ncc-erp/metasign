import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignServerDataService {
  _propertiesCanBeAdded = new BehaviorSubject<{ key: string, editable: boolean }[]>([]);
  _propertiesCanBeAdded$ = this._propertiesCanBeAdded.asObservable();
  constructor() { }
}
