import { useMemo, useState } from 'react';
import './IdeasPage.css';

const TO_EMAIL = 'israel.crawford2011@gmail.com';

type Priority = 'Low' | 'Medium' | 'High';
type IdeaType = 'Feature' | 'Improvement' | 'Integration' | 'Bug' | 'Content';

type Scenario = { given: string; when: string; then: string };

export function IdeasPage() {
  // Basic fields
  const [title, setTitle] = useState('');
  const [who, setWho] = useState('');
  const [problem, setProblem] = useState('');
  const [desiredOutcome, setDesiredOutcome] = useState('');
  const [priority, setPriority] = useState<Priority>('Medium');
  const [ideaType, setIdeaType] = useState<IdeaType>('Feature');
  const [timeSensitivity, setTimeSensitivity] = useState<string>('');
  const [workaround, setWorkaround] = useState('');

  // Lists
  const [scenarios, setScenarios] = useState<Scenario[]>([
    { given: '', when: '', then: '' }
  ]);
  const [mustHaves, setMustHaves] = useState<string>('');
  const [niceToHaves, setNiceToHaves] = useState<string>('');
  const [assumptions, setAssumptions] = useState<string>('');
  const [links, setLinks] = useState<string>('');

  // Contact & consent
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [consent, setConsent] = useState(false);

  const [toast, setToast] = useState<string | null>(null);

  const mustList = useMemo(() => splitLines(mustHaves), [mustHaves]);
  const niceList = useMemo(() => splitLines(niceToHaves), [niceToHaves]);
  const assumptionsList = useMemo(() => splitLines(assumptions), [assumptions]);
  const linkList = useMemo(() => splitLines(links), [links]);

  function splitLines(text: string): string[] {
    return text
      .split(/\r?\n/)
      .map(s => s.trim())
      .filter(Boolean);
  }

  function addScenario() {
    setScenarios(prev => [...prev, { given: '', when: '', then: '' }]);
  }

  function updateScenario(index: number, field: keyof Scenario, value: string) {
    setScenarios(prev => prev.map((s, i) => i === index ? { ...s, [field]: value } : s));
  }

  function removeScenario(index: number) {
    setScenarios(prev => prev.filter((_, i) => i !== index));
  }

  function validate(): string | null {
    if (!title.trim()) return 'Title is required';
    if (!who.trim()) return 'Who benefits is required';
    if (!problem.trim()) return 'Problem is required';
    if (!desiredOutcome.trim()) return 'Desired outcome is required';
    if (!name.trim()) return 'Your name is required';
    if (!email.trim() || !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) return 'A valid email is required';
    if (!consent) return 'Consent is required to proceed';
    return null;
  }

  function yamlEscape(value: string): string {
    const v = value.replace(/"/g, '\\"');
    return v;
  }

  function buildYaml(): string {
    const ts = new Date().toISOString();
    const scenariosYaml = scenarios
      .filter(s => s.given || s.when || s.then)
      .map(s => `  - given: "${yamlEscape(s.given)}"\n    when: "${yamlEscape(s.when)}"\n    then: "${yamlEscape(s.then)}"`)
      .join('\n');

    const list = (arr: string[]) => arr.map(s => `  - "${yamlEscape(s)}"`).join('\n');

    return `version: v1\n` +
`metadata:\n` +
`  submittedAt: ${ts}\n` +
`  source: ideas-form\n` +
`  contact:\n` +
`    name: "${yamlEscape(name)}"\n` +
`    email: "${yamlEscape(email)}"\n` +
`idea:\n` +
`  title: "${yamlEscape(title)}"\n` +
`  type: ${ideaType}\n` +
`  priority: ${priority}\n` +
`  timeSensitivity: ${timeSensitivity ? '"' + yamlEscape(timeSensitivity) + '"' : 'null'}\n` +
`problem:\n` +
`  who: "${yamlEscape(who)}"\n` +
`  problem: "${yamlEscape(problem)}"\n` +
`  currentWorkaround: ${workaround ? '"' + yamlEscape(workaround) + '"' : 'null'}\n` +
`desiredOutcome: "${yamlEscape(desiredOutcome)}"\n` +
`scenarios:${scenariosYaml ? '\n' + scenariosYaml : ' []'}\n` +
`requirements:\n` +
`  mustHaves:${mustList.length ? '\n' + list(mustList) : ' []'}\n` +
`  niceToHaves:${niceList.length ? '\n' + list(niceList) : ' []'}\n` +
`constraints:\n` +
`  assumptions:${assumptionsList.length ? '\n' + list(assumptionsList) : ' []'}\n` +
`  links:${linkList.length ? '\n' + list(linkList) : ' []'}\n`;
  }

  function buildHumanSummary(): string {
    const lines: string[] = [];
    lines.push(`Title: ${title}`);
    lines.push(`Type: ${ideaType} | Priority: ${priority} | Time sensitivity: ${timeSensitivity || 'None'}`);
    lines.push(`Submitter: ${name} (${email})`);
    lines.push('');
    lines.push('Why this matters');
    lines.push(`- Who: ${who}`);
    lines.push(`- Problem: ${problem}`);
    lines.push(`- Desired outcome: ${desiredOutcome}`);
    const sc = scenarios.filter(s => s.given || s.when || s.then);
    if (sc.length) {
      lines.push('');
      lines.push('Scenarios (Given/When/Then)');
      sc.forEach(s => lines.push(`- ${s.given} | ${s.when} | ${s.then}`));
    }
    if (mustList.length || niceList.length || assumptionsList.length || linkList.length) {
      lines.push('');
      lines.push('Requirements');
      if (mustList.length) lines.push(`- Must-haves: ${mustList.join('; ')}`);
      if (niceList.length) lines.push(`- Nice-to-haves: ${niceList.join('; ')}`);
      if (assumptionsList.length) lines.push(`- Constraints/assumptions: ${assumptionsList.join('; ')}`);
      if (linkList.length) lines.push(`- Links: ${linkList.join(' ')}`);
    }
    return lines.join('\n');
  }

  function urlEncode(s: string) {
    return encodeURIComponent(s);
  }

  async function onOpenEmail() {
    const error = validate();
    if (error) {
      setToast(error);
      setTimeout(() => setToast(null), 3000);
      return;
    }

    const summary = buildHumanSummary();
    const yaml = buildYaml();
    const fullBody = `${summary}\n\nMachine-readable payload (YAML)\n\n${yaml}`;

    // Always try to copy full content
    try {
      await navigator.clipboard.writeText(fullBody);
      setToast('Full content copied to clipboard — paste into the email if needed.');
      setTimeout(() => setToast(null), 3500);
    } catch {
      // Non-blocking
    }

    const subject = `[New Idea] ${title} (Priority: ${priority})`;

    // Safely attempt mailto with possibly shortened body
    const mailtoLimit = 1800; // conservative limit for encoded URI length
    const encodedFull = urlEncode(fullBody);
    const ccParam = email ? `&cc=${urlEncode(email)}` : '';

    let bodyForUrl = fullBody;
    if (`to=${TO_EMAIL}&subject=${urlEncode(subject)}${ccParam}&body=${encodedFull}`.length > mailtoLimit) {
      bodyForUrl = 'My full idea content is copied to the clipboard. Please paste it here.\n\n' + summary;
    }

    const mailtoUrl = `mailto:${TO_EMAIL}?subject=${urlEncode(subject)}${ccParam}&body=${urlEncode(bodyForUrl)}`;

    // Open default mail client
    window.location.href = mailtoUrl;
  }

  const gmailUrl = useMemo(() => {
    const subject = `[New Idea] ${title || '(no title)'} (Priority: ${priority})`;
    const preview = buildHumanSummary();
    return `https://mail.google.com/mail/?view=cm&fs=1&to=${encodeURIComponent(TO_EMAIL)}&cc=${encodeURIComponent(email)}&su=${encodeURIComponent(subject)}&body=${encodeURIComponent(preview + '\n\n(Full content copied to clipboard — paste here)')}`;
  }, [title, priority, email, who, problem, desiredOutcome, scenarios, mustHaves, niceToHaves, assumptions, links, name]);

  const outlookUrl = useMemo(() => {
    const subject = `[New Idea] ${title || '(no title)'} (Priority: ${priority})`;
    const preview = buildHumanSummary();
    return `https://outlook.office.com/mail/deeplink/compose?to=${encodeURIComponent(TO_EMAIL)}&subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(preview + '\n\n(Full content copied to clipboard — paste here)')}`;
  }, [title, priority, email, who, problem, desiredOutcome, scenarios, mustHaves, niceToHaves, assumptions, links, name]);

  return (
    <div className="ideas-page">
      <div className="ideas-intro form-card">
        <h2>Tell me what you want to see developed</h2>
        <p>Describe your idea below. On "Open Email", your default mail app opens with a pre-filled message — and the full content is copied to your clipboard in case the email app trims it.</p>
        <p className="small-hint">Privacy: nothing is sent anywhere until you send the email.</p>
      </div>

      <div className="ideas-form">
        <div className="ideas-main">
          <div className="form-card">
            <h3>About your idea</h3>
            <div className="form-group">
              <label htmlFor="title">Title *</label>
              <input id="title" type="text" value={title} onChange={e => setTitle(e.target.value)} required />
            </div>
            <div className="form-group">
              <label htmlFor="who">Who benefits? *</label>
              <input id="who" type="text" value={who} onChange={e => setWho(e.target.value)} required />
            </div>
            <div className="form-group">
              <label htmlFor="problem">What problem are you trying to solve? *</label>
              <textarea id="problem" rows={3} value={problem} onChange={e => setProblem(e.target.value)} required />
            </div>
            <div className="form-group">
              <label htmlFor="outcome">Desired outcome *</label>
              <textarea id="outcome" rows={2} value={desiredOutcome} onChange={e => setDesiredOutcome(e.target.value)} required />
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="priority">Priority *</label>
                <select id="priority" value={priority} onChange={e => setPriority(e.target.value as Priority)}>
                  <option>Low</option>
                  <option>Medium</option>
                  <option>High</option>
                </select>
              </div>
              <div className="form-group">
                <label htmlFor="type">Type</label>
                <select id="type" value={ideaType} onChange={e => setIdeaType(e.target.value as IdeaType)}>
                  <option>Feature</option>
                  <option>Improvement</option>
                  <option>Integration</option>
                  <option>Bug</option>
                  <option>Content</option>
                </select>
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="time">Time sensitivity</label>
                <input id="time" type="text" placeholder="e.g., By end of Q1" value={timeSensitivity} onChange={e => setTimeSensitivity(e.target.value)} />
              </div>
              <div className="form-group">
                <label htmlFor="workaround">Current workaround</label>
                <input id="workaround" type="text" value={workaround} onChange={e => setWorkaround(e.target.value)} />
              </div>
            </div>
          </div>

          <div className="form-card">
            <h3>Scenarios (Given / When / Then)</h3>
            {scenarios.map((s, i) => (
              <div className="form-row" key={i}>
                <div className="form-group">
                  <label htmlFor={`given-${i}`}>Given</label>
                  <input id={`given-${i}`} type="text" value={s.given} onChange={e => updateScenario(i, 'given', e.target.value)} />
                </div>
                <div className="form-group">
                  <label htmlFor={`when-${i}`}>When</label>
                  <input id={`when-${i}`} type="text" value={s.when} onChange={e => updateScenario(i, 'when', e.target.value)} />
                </div>
                <div className="form-group">
                  <label htmlFor={`then-${i}`}>Then</label>
                  <input id={`then-${i}`} type="text" value={s.then} onChange={e => updateScenario(i, 'then', e.target.value)} />
                </div>
                {i > 0 && (
                  <div className="form-group">
                    <button type="button" className="btn-secondary" onClick={() => removeScenario(i)}>Remove</button>
                  </div>
                )}
              </div>
            ))}
            <button type="button" className="btn-secondary" onClick={addScenario}>Add Scenario</button>
          </div>

          <div className="form-card">
            <h3>Requirements</h3>
            <div className="form-group">
              <label htmlFor="must">Must-haves (one per line)</label>
              <textarea id="must" rows={3} value={mustHaves} onChange={e => setMustHaves(e.target.value)} />
            </div>
            <div className="form-group">
              <label htmlFor="nice">Nice-to-haves (one per line)</label>
              <textarea id="nice" rows={3} value={niceToHaves} onChange={e => setNiceToHaves(e.target.value)} />
            </div>
            <div className="form-group">
              <label htmlFor="assumptions">Constraints / assumptions (one per line)</label>
              <textarea id="assumptions" rows={3} value={assumptions} onChange={e => setAssumptions(e.target.value)} />
            </div>
            <div className="form-group">
              <label htmlFor="links">Links (one per line)</label>
              <textarea id="links" rows={3} value={links} onChange={e => setLinks(e.target.value)} />
            </div>
          </div>
        </div>

        <div className="ideas-side">
          <div className="form-card sticky">
            <h3>Contact & consent</h3>
            <div className="form-group">
              <label htmlFor="name">Your name *</label>
              <input id="name" type="text" value={name} onChange={e => setName(e.target.value)} required />
            </div>
            <div className="form-group">
              <label htmlFor="email">Your email *</label>
              <input id="email" type="email" value={email} onChange={e => setEmail(e.target.value)} required />
            </div>
            <div className="form-group">
              <label>
                <input type="checkbox" checked={consent} onChange={e => setConsent(e.target.checked)} /> I agree to be contacted about this idea.
              </label>
            </div>

            <div className="buttons">
              <button type="button" className="btn-primary" onClick={onOpenEmail}>Open Email</button>
              <div className="inline-links">
                <a href={gmailUrl} target="_blank" rel="noreferrer">Open in Gmail</a>
                <a href={outlookUrl} target="_blank" rel="noreferrer">Open in Outlook Web</a>
              </div>
              {toast && <div className="toast" role="status" aria-live="polite">{toast}</div>}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
