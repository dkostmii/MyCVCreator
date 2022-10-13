const prefLen = 12;

const headers = document.querySelectorAll(".main-container header");

for (const header of headers) {
  const firstChild = header.firstElementChild;
  firstChild.innerHTML += "&nbsp;".repeat(prefLen - firstChild.innerHTML.length)
}

const cornerDecorations = new Array(2).fill(0).map(() => {
  const el = document.createElement('div');
  el.className = 'corner-decoration';

  return el;
});

const getValue = (strValue) => {
  const pattern = /[^\d\.]/gm;
  const result = parseFloat(strValue.replace(pattern, ""));

  return result;
}

const createObjectFilter = (keys, valOperand) => obj => keys.reduce((acc, key) => {
  if (valOperand instanceof Function) {
    if (key in obj) {
      acc[key] = valOperand(obj[key]);
    }
  }
  else {
    acc[key] = obj[key];
  }
  return acc;
}, {});

if (window.getComputedStyle(document.body).boxSizing !== "content-box") {
  document.body.style.boxSizing = 'content-box';
}

const filter = createObjectFilter(
  [ 'paddingLeft', 'paddingTop', 'width', 'height' ], getValue);

const selected = filter(window.getComputedStyle(document.body));
selected.height += selected.paddingTop * 2;
selected.width += selected.paddingLeft * 2;

const { paddingLeft, paddingTop, width: bodyWidth, height: bodyHeight } = selected;

const mmToPx = (milimeters) => {
  const factor = bodyWidth / 210;
  return milimeters * factor;
};

const verticalSizingFilter = createObjectFilter(
  [ 'marginTop', 'marginBottom', 'height' ], getValue);

const bodyHeader = document.querySelector("body > header");

const selectedHeader = verticalSizingFilter(window.getComputedStyle(bodyHeader), getValue);
const [ firstChild, lastChild ] = [
  bodyHeader.firstElementChild,
  bodyHeader.lastElementChild
]
.map(el => window.getComputedStyle(el))
.map(el => verticalSizingFilter(el, getValue));

const leftElement = document.querySelectorAll(".main-container")[0].firstElementChild;
const selectedLeftElement = verticalSizingFilter(window.getComputedStyle(leftElement), getValue);
const freeVerticalSpace = (
  bodyHeight - 2 * paddingTop - firstChild.marginTop - selectedHeader.height - lastChild.marginBottom - selectedLeftElement.height);

const spacing = mmToPx(getValue("12mm"));
let id = 0;
const factor = 0.75;

for (const cornerDecoration of cornerDecorations) {
  const [cornerMarginTop, cornerMarginLeft] = ["25mm", "0"].map(val => mmToPx(getValue(val)));

  const scaledHeight = (freeVerticalSpace - cornerMarginTop) * Math.pow(factor, id);
  const scaledWidth = (freeVerticalSpace - cornerMarginLeft) * Math.pow(factor, id);

  const computedTop = bodyHeight - paddingTop - scaledHeight - id * spacing;
  const computedLeft = paddingLeft + id * spacing + cornerMarginLeft;

  Object.assign(cornerDecoration.style, {
    width: `${scaledWidth}px`,
    height: `${scaledHeight}px`,
    position: "absolute",
    top: `${computedTop}px`,
    left: `${computedLeft}px`,
    marginTop: '0.1px'
  });
  id++;
  document.body.appendChild(cornerDecoration);
}
