import React, { useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import './styles.css';

const API = '';
const money = v => new Intl.NumberFormat('es-PE',{style:'currency',currency:'PEN'}).format(v);
const dt = v => new Date(v).toLocaleString('es-PE',{dateStyle:'medium',timeStyle:'short'});

function App(){
  const [tab,setTab]=useState('search');
  const [filters,setFilters]=useState({origin:'Tacna',destination:'Lima',date:''});
  const [flights,setFlights]=useState([]); const [loading,setLoading]=useState(false);
  const [selected,setSelected]=useState(null); const [message,setMessage]=useState('');
  const [userId,setUserId]=useState(localStorage.getItem('flight-user')||'demo-user');
  const [reservations,setReservations]=useState([]);
  const [form,setForm]=useState({passengerName:'',passengerEmail:'',seatNumber:'1A'});

  async function search(e){e?.preventDefault();setLoading(true);setMessage('');try{const p=new URLSearchParams();Object.entries(filters).forEach(([k,v])=>v&&p.set(k,v));const r=await fetch(`${API}/flights?${p}`);setFlights(await r.json());}catch{setMessage('No se pudo consultar los vuelos.');}finally{setLoading(false)}}
  async function loadReservations(id=userId){const normalized=id.trim();if(!normalized){setReservations([]);return;} localStorage.setItem('flight-user',normalized); try{const r=await fetch(`${API}/reservations/${encodeURIComponent(normalized)}`); if(!r.ok) throw new Error(`HTTP ${r.status}`); setReservations(await r.json());}catch(err){console.error(err);setMessage('No se pudieron cargar las reservas.');}}
  async function reserve(e){
    e.preventDefault();
    setMessage('');
    try{
      const payload={...form,seatNumber:form.seatNumber.trim().toUpperCase(),userId:userId.trim(),flightId:selected.id};
      const r=await fetch(`${API}/reservations`,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(payload)});
      const text=await r.text();
      let data=null;
      try{data=text?JSON.parse(text):null;}catch{}
      if(!r.ok){setMessage(data?.message||`No se pudo reservar (HTTP ${r.status}).`);return;}
      setUserId(payload.userId);
      localStorage.setItem('flight-user',payload.userId);
      setMessage(`Reserva confirmada para el asiento ${payload.seatNumber}.`);
      setSelected(null);
      await loadReservations(payload.userId);
      setTab('reservations');
    }catch(err){
      console.error(err);
      setMessage('Ocurrió un error al crear o cargar la reserva.');
    }
  }
  async function cancel(id){if(!confirm('¿Cancelar esta reserva?'))return;await fetch(`${API}/reservations/${id}`,{method:'DELETE'});await loadReservations();}
  async function editSeat(r){const seat=prompt('Nuevo asiento (ej. 4C):',r.seatNumber);if(!seat)return;const res=await fetch(`${API}/reservations/${r.id}`,{method:'PUT',headers:{'Content-Type':'application/json'},body:JSON.stringify({passengerName:r.passengerName,passengerEmail:r.passengerEmail,seatNumber:seat})});if(!res.ok){const x=await res.json();alert(x.message||'No se pudo modificar');}await loadReservations();}
  useEffect(()=>{search();},[]);
  useEffect(()=>{if(tab==='reservations')loadReservations(userId);},[tab]);

  return <div className="app">
    <header><div className="brand"><span className="logo">✈</span><div><strong>AeroReserva</strong><small>Vuela simple. Reserva seguro.</small></div></div><nav><button className={tab==='search'?'active':''} onClick={()=>setTab('search')}>Buscar vuelos</button><button className={tab==='reservations'?'active':''} onClick={()=>setTab('reservations')}>Mis reservas</button></nav></header>
    <main>
      {message&&<div className="notice">{message}<button onClick={()=>setMessage('')}>×</button></div>}
      {tab==='search'&&<>
        <section className="hero"><span className="eyebrow">TU PRÓXIMO DESTINO</span><h1>El viaje empieza<br/>con una buena elección.</h1><p>Busca rutas, compara horarios y asegura tu asiento en pocos pasos.</p></section>
        <form className="search-card" onSubmit={search}><label>Origen<input value={filters.origin} onChange={e=>setFilters({...filters,origin:e.target.value})} placeholder="Tacna" required/></label><span className="swap">⇄</span><label>Destino<input value={filters.destination} onChange={e=>setFilters({...filters,destination:e.target.value})} placeholder="Lima" required/></label><label>Fecha<input type="date" value={filters.date} onChange={e=>setFilters({...filters,date:e.target.value})}/></label><button className="primary">{loading?'Buscando…':'Buscar vuelos'}</button></form>
        <section className="results"><div className="section-title"><div><span className="eyebrow">RESULTADOS</span><h2>Vuelos disponibles</h2></div><span>{flights.length} opciones disponibles</span></div>{flights.map(f=><article className="flight" key={f.id}><div className="airline"><span className="airline-mark">{f.airline[0]}</span><div><strong>{f.airline}</strong><small>{f.flightNumber}</small></div></div><div className="route"><div><strong>{new Date(f.departureTime).toLocaleTimeString('es-PE',{hour:'2-digit',minute:'2-digit'})}</strong><small>{f.origin}</small></div><div className="line"><span>✈</span></div><div><strong>{new Date(f.arrivalTime).toLocaleTimeString('es-PE',{hour:'2-digit',minute:'2-digit'})}</strong><small>{f.destination}</small></div></div><div className="price"><small>Desde</small><strong>{money(f.price)}</strong><button onClick={()=>setSelected(f)}>Elegir vuelo</button></div></article>)}{!loading&&flights.length===0&&<div className="empty">No encontramos vuelos para esa búsqueda.</div>}</section>
      </>}
      {tab==='reservations'&&<section className="reservations"><div className="section-title"><div><span className="eyebrow">TU VIAJE</span><h2>Mis reservas</h2></div><div className="userbox"><input value={userId} onChange={e=>setUserId(e.target.value)} placeholder="ID de usuario"/><button onClick={loadReservations}>Cargar</button></div></div>{reservations.map(r=><article className="booking" key={r.id}><div><span className={`status ${r.status.toLowerCase()}`}>{r.status}</span><h3>{r.flight?.origin} → {r.flight?.destination}</h3><p>{r.flight?.airline} · {r.flight?.flightNumber} · {dt(r.flight?.departureTime)}</p></div><div className="seat">Asiento <strong>{r.seatNumber}</strong></div><div className="actions">{r.status==='Confirmed'&&<><button onClick={()=>editSeat(r)}>Modificar</button><button className="danger" onClick={()=>cancel(r.id)}>Cancelar</button></>}</div></article>)}{reservations.length===0&&<div className="empty">Aún no hay reservas para este usuario.</div>}</section>}
    </main>
    {selected&&<div className="modal-bg" onClick={e=>e.target===e.currentTarget&&setSelected(null)}><div className="modal"><button className="close" onClick={()=>setSelected(null)}>×</button><span className="eyebrow">CONFIRMAR RESERVA</span><h2>{selected.origin} → {selected.destination}</h2><p>{selected.airline} · {selected.flightNumber} · {dt(selected.departureTime)}</p><form onSubmit={reserve}><label>ID de usuario<input value={userId} onChange={e=>setUserId(e.target.value)} required/></label><label>Pasajero<input value={form.passengerName} onChange={e=>setForm({...form,passengerName:e.target.value})} required/></label><label>Correo<input type="email" value={form.passengerEmail} onChange={e=>setForm({...form,passengerEmail:e.target.value})} required/></label><label>Asiento<input value={form.seatNumber} pattern="[1-9][0-9]?[A-Fa-f]" onChange={e=>setForm({...form,seatNumber:e.target.value})} required/></label><div className="summary"><span>Total</span><strong>{money(selected.price)}</strong></div><button className="primary full">Confirmar reserva</button></form></div></div>}
    <footer>Proyecto académico · API REST .NET 8 · React · PostgreSQL</footer>
  </div>
}

createRoot(document.getElementById('root')).render(<App/>);
