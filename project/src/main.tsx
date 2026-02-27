import { render } from 'preact';

import preactLogo from './assets/preact.svg';
import ResponsiveDrawer from './menu';

export function App() {
	return (
		<div>
			<ResponsiveDrawer/>
		</div>
	);
}

function Resource(props) {
	return (
		<a href={props.href} target="_blank" class="resource">
			<h2>{props.title}</h2>
			<p>{props.description}</p>
		</a>
	);
}

render(<App />, document.getElementById('app'));
