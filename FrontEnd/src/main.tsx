import { render } from 'preact';

import Menu from './menu';

export function App() {
	return (
		<div>
			<Menu/>
		</div>
	);
}

render(<App />, document.getElementById('app'));
