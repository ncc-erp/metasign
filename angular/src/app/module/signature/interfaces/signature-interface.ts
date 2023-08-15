export interface IInput {
    positionY: number;
    positionX: number;
    value: string;
    color: string;
    fontFamily: string;
    fontSize: number;
    canFillInput: boolean;
    width: number;
    height: number;
    onDrag: false;
    isResized?: boolean;
}
