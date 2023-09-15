import { buttomType } from './../../AppEnums';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-buttom-login',
  templateUrl: './buttom-login.component.html',
  styleUrls: ['./buttom-login.component.css']
})
export class ButtomLoginComponent implements OnInit {

  @Input() text: string;
  @Input() type: buttomType;
  buttomType = buttomType
  constructor() { }

  ngOnInit() {
  }

}
